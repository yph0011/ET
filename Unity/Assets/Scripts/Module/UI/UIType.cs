﻿using System;
using System.Collections.Generic;

namespace ETModel
{
    public static class UIType
    {
	    public const int Root = 0;
	    public const int UILoading = 10000;

	    public static Dictionary<int, string> UIName = new Dictionary<int, string>()
	    {
			{Root,              "Root" },
			{UILoading,         "UILoading" },
	    };

	    public static string GetUIName(int type)
	    {
		    try
		    {
			    return UIName[type];
		    }
		    catch (Exception e)
		    {
			    throw new Exception($"not found ui: {type}", e);
		    }
	    }
	}
}