using System;

/// <summary>
/// Represents a heap data structure
/// </summary>
/// <typeparam name="T"> What type this heap holds </typeparam>
public class Heap<T> where T : IHeapItem<T>
{
    // array of items in the heap
    private T[] items;
    
    // number of items in the heap
    private int _count;
    public int Count => _count;

    /// <summary>
    /// Constructor for a heap
    /// </summary>
    /// <param name="maxHeapSize"> Maximum number of elements this heap can store </param>
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    /// <summary>
    /// Add an item to the heap
    /// </summary>
    /// <param name="item"> Item to add to the heap </param>
    public void Add(T item)
    {
        item.heapIndex = _count;
        items[_count] = item;
        SortUp(item);
        _count++;
    }

    /// <summary>
    /// Remove the first item from the heap
    /// </summary>
    /// <returns> The first item which was removed </returns>
    public T RemoveFirst()
    {
        T firstItem = items[0];
        _count--;
        items[0] = items[_count];
        items[0].heapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    /// <summary>
    /// Update an item's position within the heap to be correct, might be used after increasing or decreasing the priority of an item
    /// </summary>
    /// <param name="item"> Item to update/refresh </param>
    public void UpdateItem(T item)
    {
        SortUp(item);
        SortDown(item);
    } 

    /// <summary>
    /// Does this heap contain the given item?
    /// </summary>
    /// <param name="item"> Item to search for </param>
    /// <returns> True if the heap contains the item, false otherwise </returns>
    public bool Contains(T item)
    {
        return Equals(items[item.heapIndex], item);
    }

    /// <summary>
    /// Sort from the item down (its children) within the heap
    /// </summary>
    /// <param name="item"> Item to sort </param>
    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.heapIndex * 2 + 1;
            int childIndexRight = item.heapIndex * 2 + 2;
            int swapIndex = 0;

            // is there a left child?
            if (childIndexLeft < _count)
            {
                swapIndex = childIndexLeft;

                // is there also a right child?
                if (childIndexRight < _count)
                {
                    // compare children to see which should take this spot
                    if (childIndexRight < _count)
                    {
                        // need to compare children to see if right or left should take priority
                        if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(items[swapIndex]) < 0)
                    {
                        Swap(item, items[swapIndex]);
                    }
                    else
                    {
                        // parent has higher priority than both its children, it is in the right place
                        return;
                    }
                }
                else
                {
                    // parent has no right child, nothing else to do
                    return;
                }
            }
            else
            {
                // no children
                return;
            }
        }
    }

    /// <summary>
    /// Sort from the item up (its parents) within the heap
    /// </summary>
    /// <param name="item"> Item to sort </param>
    void SortUp(T item)
    {
        int parentIndex = (item.heapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
            
            parentIndex = (item.heapIndex - 1) / 2;
        }
    }

    /// <summary>
    /// Swap two items in the heap
    /// </summary>
    /// <param name="itemA"> First item to swap with </param>
    /// <param name="itemB"> Second item to swap with </param>
    void Swap(T itemA, T itemB)
    {
        items[itemA.heapIndex] = itemB;
        items[itemB.heapIndex] = itemA;
        
        // swap their HeapIndex with each other
        (itemA.heapIndex, itemB.heapIndex) = (itemB.heapIndex, itemA.heapIndex);
    }
}

/// <summary>
/// Represents an item in the heap. Remembers its position.
/// </summary>
/// <typeparam name="T"> The item's type </typeparam>
public interface IHeapItem<T> : IComparable<T>
{
    int heapIndex { get; set; }
}