using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<T> where T : IComparable<T>
{
    List<T> storage = new List<T>();
	int size = 0; 
    int GetSmallerChild(int index)
    {
        int index1 = index * 2 + 1;
        int index2 = index * 2 + 2;
        if (index1 >= storage.Count)
        {
            return -1;
        }
        if (index2 == storage.Count)
        {
            return index1;
        }
        var child1 = storage[index1];
        var child2 = storage[index2];
		return child1.CompareTo(child2) <= 0 ? index1 : index2;
    }
    public T Peek()
    {
        if (storage.Count > 0)
        {
            return storage[0];
        }
        return default(T);
    }
    public void Remove(T item)
    {
        var index = storage.IndexOf(item);
        if (index >= 0)
        {
            Remove(index);
        }
    }
    public T Remove(int index = 0)
    {
		size--; 
        if (storage.Count == 0)
        {
            return default(T);
        }
        var result = storage[index];
        Swap(index, storage.Count - 1);
        storage.RemoveAt(storage.Count - 1);
        BubbleDown(index);
        return result;
    }
    public void Insert(T val)
    {
		size++; 
        storage.Add(val);
        BubbleUp();
    }
    void Swap(int index1, int index2)
    {
        var temp = storage[index1];
        storage[index1] = storage[index2];
        storage[index2] = temp;
    }
    void BubbleDown(int currentIndex = 0)
    {
        while (currentIndex < storage.Count)
        {
            var childIndex = GetSmallerChild(currentIndex);
            if (childIndex == -1)
            {
                return;
            }
            var parent = storage[currentIndex];
            var child = storage[childIndex];
            if (parent.CompareTo(child) > 0)
            {
                Swap(currentIndex, childIndex);
                currentIndex = childIndex;
            }
            else
            {
                return;
            }
        }
    }
    void BubbleUp()
    {
        int currentIndex = storage.Count - 1;
        while (currentIndex > 0)
        {
            var parentIndex = Mathf.FloorToInt((currentIndex - 1f) / 2f);
            if (storage[parentIndex].CompareTo(storage[currentIndex]) > 0)
            {
                Swap(parentIndex, currentIndex);
                currentIndex = parentIndex;
            }
            else
            {
                return;
            }
        }
    }

}
