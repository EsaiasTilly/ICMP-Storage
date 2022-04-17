class Range {
  public int start;
  public int end;
  public Range(int s, int e) {
    start = s;
    end = e;
  }

  public static List<Range> combineRanges(List<Range> ranges) {
    List<Range> combined = new List<Range>();
    for (int i = 0; i < ranges.Count; i++) {
      Range r = ranges[i];
      if (combined.Count == 0) {
        combined.Add(r);
      } else {
        Range last = combined[combined.Count - 1];
        if (r.start <= last.end) {
          last.end = r.end > last.end ? r.end : last.end;
        } else {
          combined.Add(r);
        }
      }
    }
    return combined;
  }

  public bool overlaps(Range other) {
    return start <= other.end && end >= other.start;
  }
}