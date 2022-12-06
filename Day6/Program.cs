var index = 0;
var bSize = 14;
var b = new Queue<char>();

foreach (var c in File.ReadAllText("data.txt"))
{
	index++;
	b.Enqueue(c) ;

	if (b.Count() < bSize)
		continue;

	if (b.Count() > bSize)
		b.Dequeue();

	if (b.GroupBy(x => x).Count() == bSize)
		break;
}

Console.WriteLine(index);