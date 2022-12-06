var data = File.ReadAllText("data.txt").Split(Environment.NewLine)
    .Select(x => x.Split(",").Select(x => x.Split("-").Select(x => int.Parse(x))))
    .Select(x => x.Select(x0 => new Range(x0.First(), x0.Last())))
    .Select(x => (x.First(), x.Last()));

bool Contains(Range range0, Range range1) =>
    range0.Lower >= range1.Lower && range0.Upper <= range1.Upper;

bool Overlap(Range range0, Range range1) =>
    range0.Upper >= range1.Lower && range0.Lower <= range1.Upper;

bool IsOverlap(Range range0, Range range1) =>
    Contains(range0, range1) || Contains(range1, range0) ||
        Overlap(range0, range1) || Overlap(range1, range0);
    
var overlaps = data.Where(x => IsOverlap(x.Item1, x.Item2));

Console.WriteLine(overlaps.Count());

record Range (int Lower, int Upper);