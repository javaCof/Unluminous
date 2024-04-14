using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static void SetCursorLock(bool cursorLock)
    {
        Cursor.lockState = cursorLock ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
