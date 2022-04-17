using System.Text;

class Read {
  // Event is invoked when a read is completed
  public delegate void EventHandler(byte[] data);
  public event EventHandler? OnReceive;

  // Read range
  public Range range;

  // Read buffer
  public byte[] data;

  // List of affected blocks
  public List<int> affected_blocks = new List<int>();

  // Indicates if the read has been completed
  private bool compelted = false;

  // Indicates when the read was started
  private double start_time;

  // Indicates if the read is busy
  private bool busy = false;

  // Constructor
  public Read(int offset, int length) {
    this.range = new Range(offset, offset + length);
    this.data = new byte[length];

    // Add all affected blocks
    var read_range = new Range(offset, offset + length);
    foreach (var block in Config.blocks) {
      var block_range = new Range(block.start, block.start + block.size);

      if (block_range.overlaps(read_range)) {
        affected_blocks.Add(block.id);
      }
    }

    // Set the start time
    start_time = DateTime.Now.TimeOfDay.TotalMilliseconds;
  }

  public byte[] waitForReceive() {
    while (!compelted &&  DateTime.Now.TimeOfDay.TotalMilliseconds - start_time <= Config.READ_TIMEOUT) {
      Thread.Sleep(150);
    }
    return data;
  }

  public bool addData(int block_id, byte[] data, int offset) {
    if (compelted || !affected_blocks.Contains(block_id)) return false;

    while(busy) Thread.Sleep(50);
    this.busy = true;

    int writeIndex = offset > this.range.start ? offset - this.range.start : 0;
    int readIndex = offset > this.range.start ? 0 : this.range.start - offset;

    for (int i = 0; writeIndex < this.data.Length && readIndex < data.Length; writeIndex++, readIndex++, i++) {
      this.data[writeIndex] = data[readIndex];
    }

    affected_blocks.Remove(block_id);

    if (this.affected_blocks.Count == 0 && !this.compelted) {
      OnReceive?.Invoke(this.data);
      this.compelted = true;
      Config.reads.Remove(this);
      this.busy = false;
      return true;
    }

    this.busy = false;
    return false;
  }
}