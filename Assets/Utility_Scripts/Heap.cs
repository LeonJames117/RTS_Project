using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Heap<T> where T: Heap_Item_Interface<T>
{
    T[] Items;
    int Curent_Index_Count;
    public int Count { get { return Curent_Index_Count; } }
    public Heap(int Max_Size)
    {
        Items = new T[Max_Size];
    }

    public void UpdateItem(T Item)
    {
        Sort_Up(Item);
        //Sort_Down(Item);
    }

    public void Add(T Item) // Add a new item to the heap
    {
        Item.Heap_Index = Curent_Index_Count;
        Items[Curent_Index_Count] = Item;
        Sort_Up(Item);
        Curent_Index_Count++;
    }


    void Sort_Up(T item)
    {
        int parentIndex = (item.Heap_Index - 1) / 2;

        while (true)
        {
            T parentItem = Items[parentIndex];
            if (item.CompareTo(parentItem) > 0) //Higher priority = 1, Same = 0, Lower = -1
            {
                Swap(item, parentItem);
            }
            else
            {
                //Debug.Log("Break Sort up");
                break;
            }

            parentIndex = (item.Heap_Index - 1) / 2;
        }
    }

    void Sort_Down(T Item) //Sort an item upwards into the heap by comparing child values
    {
        while (true)
        {
            int Left_Child_Index = Item.Heap_Index * 2 + 1;
            int Right_Child_Index = Item.Heap_Index * 2 + 2;
            int Swap_Ind=0;
            if(Left_Child_Index < Curent_Index_Count) //If Item has children
            {
                Swap_Ind = Left_Child_Index;
                if(Right_Child_Index < Curent_Index_Count)
                {
                    if (Items[Left_Child_Index].CompareTo(Items[Right_Child_Index]) < 0) //If right child has higher priority
                    {
                        Swap_Ind = Right_Child_Index;
                    }
                }

                if (Item.CompareTo(Items[Swap_Ind]) < 0) //If the highest priority child has higher priority than parent
                {
                    Swap(Item, Items[Swap_Ind]);
                }
                else //Parent has highest priority, does not need to be swapped
                {
                    return;
                }
            }
            else //Parent has no children, Does not need to be swapped
            {
                return;
            }

        }
    }

    void Swap(T itemA, T itemB)
    {
        Items[itemA.Heap_Index] = itemB;
        Items[itemB.Heap_Index] = itemA;
        int itemAIndex = itemA.Heap_Index;
        itemA.Heap_Index = itemB.Heap_Index;
        itemB.Heap_Index = itemAIndex;
    }

    public T Remove_First_Item()
    {
        T First = Items[0];
        Curent_Index_Count--;
        Items[0] = Items[Curent_Index_Count];
        Items[0].Heap_Index = 0;
        Sort_Down(Items[0]);
        return First;
    }

    public bool Contains(T Item)
    {
        return Equals(Items[Item.Heap_Index], Item);
    }

    

}

public interface Heap_Item_Interface<T> : IComparable<T>
{
    int Heap_Index { get; set; }

}