var numbers = System.IO.File.ReadAllLines("data.txt")
	.Select(x => int.Parse(x))
	.Select(x => new Number(x))
	.ToList();

var numbersList = new LinkedList<Number>(numbers);
var count = 1;

foreach (var num in numbers)
{
	var listItem = numbersList.Nodes().First(n => n.Value.Id == num.Id);

	var items = numbersList.Nodes().Where(n => n.Value.Id == num.Id).ToList();
	if (items.Count() > 1)
	{
			var r = 4;
	}

	if (listItem.Value.Num == 0)
		continue;

	var nextItem = listItem;

	var i = 1;
	for (;i <= Math.Abs(listItem.Value.Num); i++)
	{

		if (listItem.Value.Num > 0)
		{
			nextItem = nextItem.Next;
			if (nextItem == null)
				nextItem = numbersList.First;
		}
		else
		{
			nextItem = nextItem.Previous;
			if (nextItem == null)
				nextItem = numbersList.Last;
		}
	}

	

	if (listItem.Value.Num < 0)
	{
			nextItem = nextItem.Previous;
			if (nextItem == null)
				nextItem = numbersList.Last;
	}
	else if (nextItem.Next == null)
	{
		nextItem = numbersList.First;
		numbersList.AddBefore(nextItem, listItem);
		continue;
	}
	
numbersList.Remove(listItem);
	numbersList.AddAfter(nextItem, listItem);

	count++;
	/*Console.WriteLine($"Run: {count}");
	foreach (var num0 in numbersList)
		Console.WriteLine(num0);
		Console.WriteLine($"{listItem.Value.Num} {i}: {nextItem.Value}");*/
}

Console.WriteLine($"Run: {count}");

var zeroItem = numbersList.Nodes().First(n => n.Value.Num == 0);
var index1 = GetValue(1000);
var index2 = GetValue(2000);
var index3 = GetValue(3000);

Console.WriteLine($"3000: {index1}");
Console.WriteLine($"2000: {index2}");
Console.WriteLine($"3000: {index3}");
Console.WriteLine($"Co-ordinates: {index1 + index2 + index3}");

int GetValue(int index)
{
	var item = zeroItem;
	for (var i = 1; i <= index; i++)
	{
		item = item.Next;
		if (item == null)
			item = numbersList.First;
	}
	return item.Value.Num;
}




/*foreach (var num in numbersList)
	Console.WriteLine(num);*/

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
	public Number(int num)
	{
		Num = num;
		Id = Guid.NewGuid();
	}

	public Guid Id { get; }	

	public int Num { get; }	
};