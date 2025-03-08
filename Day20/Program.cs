using System.Numerics;

var numbers = System.IO.File.ReadAllLines("data.txt")
	.Select(x => int.Parse(x))
	.Select(x => new Number(x))
	.ToList();

var numbersList = new LinkedList<Number>(numbers);
var count = 0;
var length = numbersList.Count;

void Mix()
{
	count++;

	foreach (var num in numbers)
	{
		var listItem = numbersList.Nodes().Single(n => n.Value.Id == num.Id);

		if (listItem.Value.Num == 0)
			continue;

		var nextItem = listItem;

		var i = 1;
		int moves = (int)(listItem.Value.Num % (length - 1));
		for (;i <= Math.Abs(moves); i++)
		{
			if (listItem.Value.Num > 0)
			{
				nextItem = nextItem.Next;
				nextItem ??= numbersList.First;
			}
			else
			{
				nextItem = nextItem.Previous;
				nextItem ??= numbersList.Last;
			}
			if (i == 1)
			{
				numbersList.Remove(listItem);
			}
		}

		if (listItem.Value.Num > 0)
		{
			numbersList.AddAfter(nextItem, listItem);
		}
		else 
		{
			numbersList.AddBefore(nextItem, listItem);
		}
	}
}

for (var i = 0; i < 10; i++)
{
	/*Console.WriteLine($"Run: {count} ");
	foreach (var num0 in numbersList)
		Console.Write($"{num0.Num} ");

	Console.WriteLine("");*/

	Mix();
}

Console.WriteLine($"Run: {count}");

var zeroItem = numbersList.Nodes().Single(n => n.Value.Num == 0);
var index1 = GetValue(1000);
var index2 = GetValue(2000);
var index3 = GetValue(3000);

Console.WriteLine($"1000: {index1}");
Console.WriteLine($"2000: {index2}");
Console.WriteLine($"3000: {index3}");
Console.WriteLine($"Co-ordinates: {index1 + index2 + index3}");

BigInteger GetValue(int index)
{
	var item = zeroItem;
	for (var i = 1; i <= index; i++)
	{
		item = item.Next;
		item ??= numbersList.First!;
	}
	return item.Value.Num;
}

public static class LinkedListExtensions
{
	public static IEnumerable<LinkedListNode<T>> Nodes<T>(this LinkedList<T> list)
	{
		for (var node = list.First; node != null; node = node.Next)
		{
			yield return node;
		}
	}
}

class Number
{
    public Number(int num) => Num = num * (BigInteger) 811589153;

    public Guid Id { get; } = Guid.NewGuid();

    public BigInteger Num { get; }
};