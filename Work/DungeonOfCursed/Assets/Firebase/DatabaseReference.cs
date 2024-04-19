using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
public class FirebaseRef
{
    private static FirebaseRef instance;

    public static FirebaseRef Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new FirebaseRef();
            }
            return instance;
        }
    }

    public static DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
}
