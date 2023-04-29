using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Heap<T> where T: Heap_Item_Interface<T>
{
    T[] Items;
    int Count;

    public Heap(int Max_Size)
    {
        Items = new T[Max_Size];
    }


    public void Add(T Item) // Add a new item to the heap
    {
        Item.Index = Count;
        Items[Count] = Item;
        Sort_Up(Item);
        Count++;
    }


    public void Sort_Up(T Item) // Sort an item upwards into the heap by comparing parent values
    {
        int Parent_Ind = (Item.Index-1)/2;
        while (true)
        {
            T Parent = Items[Parent_Ind];
            if (Item.CompareTo(Parent) > 0) //Higher = 1, Same = 0, Lower = -1
            {
                Swap(Item, Parent);
            }
            else
            {
                break;
            }
            Parent_Ind = (Item.Index - 1) / 2;
        }
    }

    void Swap(T A, T B)
    {
        Items[A.Index] = Items[B.Index];
        Items[B.Index] = Items[A.Index];
        int ItemA_Ind = A.Index;
        A.Index = B.Index;
        B.Index = ItemA_Ind;
    }

}

public interface Heap_Item_Interface<T> : IComparable<T>
{
    int Index { get; set; }

}