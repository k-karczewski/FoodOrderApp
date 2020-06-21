﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.Enums
{
    public enum SizeEnum
    {
        Small,
        Medium,
        Big,
        Large
    }

    public enum ResultType
    {
        Correct,
        Failed,
        Error,
        Created,
        Edited,
        Deleted,
        Unauthorized
    }
}