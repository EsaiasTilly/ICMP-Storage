class FileSystem {
  /// File System data structure
  /// |     2048 B     |   4096 B   |
  /// |    Pointers    |    Data    |

  /// Directory pointer specification
  /// |  Max 99 B  |       1 B       |
  /// |    Path    |  Pointer Break  |

  /// File pointer specification
  /// | Max 99 B |      1 B      |   Max 300 B  |      1 B      |
  /// |   Path   | Pointer Seper |   Pointers   | Pointer Break |
  
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
  ///   |   16 B   |   16 B   |     1 B      |
  ///   |  Offset  |  Length  | Offset Break |

  /// Offset Break:
  /// - Breaks between data pointers have a value of 0

  /// Example pointers:
  ///  /   PoB          /home/              PoB                /home/temp                        Pointers  PoB             Filler
  /// [47] [3] [47][104][111][109][101][47] [3] [47][104][111][109][101][47][116][101][109][112] [.......] [3] [0][0][0][0][0][0][0][0][0][0][0][0]
}