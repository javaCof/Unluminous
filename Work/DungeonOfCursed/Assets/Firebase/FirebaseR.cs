using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
public class FirebaseR
{
    public static DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
}
