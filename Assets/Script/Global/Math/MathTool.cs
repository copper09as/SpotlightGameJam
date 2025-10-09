using UnityEngine;

public static class MathTool
{
    public static Vector4 SwapNumber(int hight, int left, int right, int low)//交换数字
    {
        Vector4 swaplist;
        int swap1;
        int swap2;
        swap1 = hight;
        hight = left;//上
        left = low;//左
        swap2 = right;
        right = swap1;//右
        low = swap2;
        swaplist = new Vector4(hight, left, right, low);
        return swaplist;
    }
}

