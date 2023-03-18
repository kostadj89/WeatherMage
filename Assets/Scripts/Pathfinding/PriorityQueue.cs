using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PriorityQueueExtensions
{
    public static T Find<T>(this PriorityQueue<T> queue, Predicate<T> match)
    {
        foreach (var item in queue.items)
        {
            if (match(item.Item1))
            {
                return item.Item1;
            }
        }
        return default(T);
    }
    //public static void UpdatePriority<T>(this PriorityQueue<T> queue, Tuple<T, int> item, double priority)
    //{
    //    queue.items.Find(i => item.Item1 == i.Item1 && item.Item2 == i.Item2);
    //    item.Priority = priority;
    //    queue.Enqueue(item);
    //}
}

public class PriorityQueue<T>
{
    
    // List of tuples to store items and their priorities
    internal List<Tuple<T, int>> items;

    // Constructor to initialize the list
    public PriorityQueue()
    {
        items = new List<Tuple<T, int>>();
    }

    // Enqueue method to add a new item and its priority to the queue
    public void Enqueue(T item, int priority)
    {
        items.Add(Tuple.Create(item, priority)); // Add the item and its priority to the end of the list
    }

    // Dequeue method to remove and return the item with the highest priority
    public T Dequeue()
    {
        // Find the item with the highest priority
        int bestIndex = 0; // Initialize the best index to the first item in the list
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Item2 < items[bestIndex].Item2) // Compare the priority of the current item with the priority of the best item so far
            {
                bestIndex = i; // If the current item has a higher priority, update the best index
            }
        }

        // Remove and return the item with the highest priority
        T bestItem = items[bestIndex].Item1; // Get the item itself from the tuple at the best index
        items.RemoveAt(bestIndex); // Remove the tuple at the best index from the list
        return bestItem; // Return the item itself
    }

    // Count property to get the number of items in the queue
    public int Count
    {
        get { return items.Count; }
    }

    public PriorityQueue<T> GetEnumenator()
    {
        return new PriorityQueue<T>();
    }
}
