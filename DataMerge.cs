class DataMerge {
  // Merge together two byte arrays
  public static byte[] Merge(byte[] original, byte[] newData, int offset) {
    if (newData.Length + offset > original.Length) throw new Exception("New data is too big");
    
    for (int i = 0; i < newData.Length; i++) {
      original[offset + i] = newData[i];
    }
    return original;
  }
}