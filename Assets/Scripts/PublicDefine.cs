using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PublicDefine
{
    public enum AR_MODE
    {
        NONE = -1,
        TRACKING,
        PICKING,
    }    

    // 각각 event 순서.. Scene 이라고 할 수 있음...
    public enum ROUTE
    {
        NONE = -1,
        TABLE_SETTING,
        SELECT_FOOD,
        PICKED_FOOD,
        DRESS_EVENT,
    }    
}
