using System.Text;
using System.Text.RegularExpressions;

class FileSystem {
  /// File System data structure
  /// |     2048 B     |   4096 B   |
  /// |    Pointers    |    Data    |

  /// Directory pointer specification
  /// |    1 B    |  Max 99 B  |       1 B       |
  /// | Type [68] |    Path    |  Pointer Break  |

  /// File pointer specification
  /// |    1 B    | Max 99 B |      1 B      |   Max 300 B  |      1 B      |
  /// | Type [70] |   Path   | Pointer Seper |   Pointers   | Pointer Break |
  
  /// Pointer Break:
  /// - Breaks between directories and/or files have a value of 3

  /// Pointer Seper(ator):
  /// - Seperates between paths and pointers
  /// - Has a value of 2

  /// Path:
  /// - Directories are separated by '/'
  /// - Paths are limited to 99 characters
  /// - Directory and file names can only contain A-Z a-z 0-9 _-.,
  /// - The root directory is '/'

  /// Pointer:
  /// - Pointers are indexes into the data section
  /// - A pointer consists of:
  ///   |   16 B   |   16 B   |
  ///   |  Offset  |  Length  |

  /// Example pointers:
  ///  /   PoB          /home/              PoB                /home/temp                        Pointers  PoB             Filler
  /// [47] [3] [47][104][111][109][101][47] [3] [47][104][111][109][101][47][116][101][109][112] [.......] [3] [0][0][0][0][0][0][0][0][0][0][0][0]

  public static async Task<bool> MakeDirectory(string path) {
    try
    {
      // Check if path is valid
      if (!isValidPath(path, false))
        throw new Exception("Invalid path");
      
      // Read pointers
      var fs = ReadFilePointers();

      // Check if directory already exists
      if (fs.directories.Contains(path))
        throw new Exception("Directory already exists");
      
      // Create directory
      fs.directories.Add(path);

      // Write pointers
      fs.write();

      return true;
    }
    catch (Exception e)
    {
      Console.WriteLine("[ERROR]" + e.Message);
      return false;
    }
  }

  // Checks if a path is allowed
  public static bool isValidPath(string path, bool forFile) {
    // Allow only 99 characters
    if (path.Length > 99) return false;

    // Allow only A-Z a-z 0-9 _-.,/
    if (!Regex.IsMatch(path, @"^[A-Za-z0-9_\-.,/]+$")) return false;

    // Do not allow '//'
    if (path.Contains("//")) return false;

    // Do not allow path to end with '/' for files
    if (forFile && path.EndsWith("/")) return false;

    // Allow only '/' as root
    if (path.StartsWith('/')) return true;

    return false;
  }

  // Read all pointers
  public static FileSystem ReadFilePointers() {
    // Read pointer space
    var read = new Read(0, 2048);
    Config.reads.Add(read);

    // Wait for read to be recieved
    byte[] data = read.waitForReceive();

    // Parse pointers
    return new FileSystem(data);
  }

  private static byte[] POINTER_BREAK = { 3 };
  private static byte[] POINTER_SEPERATOR = { 2 };
  private static byte DIRECTORY_TYPE = 68;
  private static byte FILE_TYPE = 70;

  public List<string> directories = new List<string>();
  public List<FilePointer> files = new List<FilePointer>();

  // Constructor
  public FileSystem(byte[] pointerData) {
    // Convert pointer data to string and split into pointers
    string data = Encoding.ASCII.GetString(pointerData);
    string[] pointers = data.Split(Encoding.ASCII.GetString(POINTER_BREAK));

    // Loop through pointers
    foreach (string pointer in pointers) {
      // Check if pointer is directory or file
      if (pointer.StartsWith(Encoding.ASCII.GetString(new byte[] { DIRECTORY_TYPE }))) AddDirectory(pointer.Substring(1));
      else if (pointer.StartsWith(Encoding.ASCII.GetString(new byte[] { FILE_TYPE }))) AddFile(pointer.Substring(1));
    }
  }

  // Converts all pointers into byte data and writes to storage
  public void write() {
    List<byte> pointers = new List<byte>();

    // Add directories
    foreach (string directory in directories) {
      // Add directory type
      pointers.Add(DIRECTORY_TYPE);

      // Add directory path
      pointers.AddRange(Encoding.ASCII.GetBytes(directory));

      // Add pointer break
      pointers.AddRange(POINTER_BREAK);
    }

    // Add files
    foreach (FilePointer file in files) {
      // Add file type
      pointers.Add(FILE_TYPE);

      // Add file path
      pointers.AddRange(Encoding.ASCII.GetBytes(file.path));

      // Add pointer seperator
      pointers.AddRange(POINTER_SEPERATOR);

      // Add file pointers
      foreach (var range in file.pointers) {
        // Create padded offset
        List<byte> offset = new List<byte>(BitConverter.GetBytes(range.start));
        while(offset.Count < 16) offset.Insert(0, 0);
        pointers.AddRange(offset);

        // Create padded length
        List<byte> length = new List<byte>(BitConverter.GetBytes(range.getLength()));
        while(length.Count < 16) length.Insert(0, 0);
        pointers.AddRange(length);
      }

      // Add pointer break
      pointers.AddRange(POINTER_BREAK);
    }

    // Pad pointers
    while(pointers.Count < 2048) pointers.Add(0);

    // Write pointers to storage
    for(int i = 0; i < 2048 / Config.BLOCK_SIZE; i++) {
      Config.blocks[i].writeToBlock(0, pointers.GetRange(i * Config.BLOCK_SIZE, Config.BLOCK_SIZE).ToArray());
    }

    // Clean up
    pointers.Clear();
    GC.Collect();
  }

  // Add a directory
  private void AddDirectory(string pointer) {
    this.directories.Add(pointer);
  }

  // Add a file
  private void AddFile(string pointer) {
    // Split the pointer into parts
    string[] parts = pointer.Split(Encoding.ASCII.GetString(POINTER_SEPERATOR));

    // Get the path
    string path = parts[0];

    // Get all pointers
    List<Range> ranges = new List<Range>();
    for (int i = 0; i < parts[1].Length / 32; i++) {
      int offset = int.Parse(parts[1].Substring(i * 32, 16));
      int length = int.Parse(parts[1].Substring(i * 32 + 16, 16));
      ranges.Add(new Range(offset, offset + length));
    }

    // Add the file
    this.files.Add(new FilePointer(path, ranges.ToArray()));
  }
}

class FilePointer {
  public string path;
  public List<Range> pointers;

  public FilePointer(string path, Range[] pointers) {
    this.path = path;
    this.pointers = new List<Range>(pointers);
  }
}