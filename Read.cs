using System.Text;

class Read {
  public delegate void EventHandler(byte[] data);
  public event EventHandler? OnReceive;
  public int offset, length;
  public byte[] data;
  public List<int> affected_blocks = new List<int>();
  private bool compelted = false;

  public Read(int offset, int length) {
    this.offset = offset;
    this.length = length;
    this.data = new byte[length];

    // Add all affected blocks
    var read_range = new Range(offset, offset + length);
    foreach (var block in Config.blocks) {
      var block_range = new Range(block.start, block.start + block.size);

      if (block_range.overlaps(read_range)) {
        affected_blocks.Add(block.id);
      }
    }
  }

  public bool addData(int block_id, byte[] data, int offset) {
    if (compelted || !affected_blocks.Contains(block_id)) return false;

    int writeIndex = offset > this.offset ? offset - this.offset : 0;
    int readIndex = offset > this.offset ? 0 : this.offset - offset;

    for (int i = 0; writeIndex < this.data.Length && readIndex < data.Length; writeIndex++, readIndex++, i++) {
      this.data[writeIndex] = data[readIndex];
    }

    affected_blocks.Remove(block_id);

    if (this.affected_blocks.Count == 0 && !this.compelted) {
      OnReceive?.Invoke(this.data);
      this.compelted = true;
      Config.reads.Remove(this);
      return true;
    }

    return false;
  }
}