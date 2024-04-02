using System.Collections.Generic;
using UnityEngine;

public static class StaticUtils
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public static void Swap<T>(this ref T src, ref T dst) where T : struct
    {
        T temp = src;
        src = dst;
        dst = temp;
    }

    public static void SwapList<T>(this List<T> list, int src, int dst)
    {
        T temp = list[src];
        list[src] = list[dst];
        list[dst] = temp;
    }

    public static void MoveTo(this List<TrainCar> list, int sourceIndex, int destinationIndex)
    {
        list.MoveTo<TrainCar>(sourceIndex, destinationIndex);
        int index = 0;
        list.ForEach(car => car.carIndex = index++);
    }

    public static void MoveTo<T>(this List<T> list, int sourceIndex, int destinationIndex)
    {
        if (sourceIndex < 0 || sourceIndex >= list.Count || destinationIndex < 0 || destinationIndex >= list.Count)
        {
            Debug.Log("인덱스가 범위를 벗어났습니다.");
            return;
        }

        // 소스 인덱스의 값을 저장
        T elementToMove = list[sourceIndex];

        // 소스 인덱스에서 제거
        list.RemoveAt(sourceIndex);

        // 대상 인덱스에 값을 삽입
        list.Insert(destinationIndex, elementToMove);
    }
    #endregion

    #region PrivateMethod
    #endregion
}