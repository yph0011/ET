﻿using System;
using UnityEngine;

namespace ETModel
{
    [UIFactory((int)UIType.UILoading)]
    public class UILoadingFactory : IUIFactory
    {
        public UI Create(Scene scene, int type, GameObject gameObject)
        {
	        try
	        {
				GameObject bundleGameObject = ((GameObject)Resources.Load("KV")).Get<GameObject>("UILoading");
				GameObject go = UnityEngine.Object.Instantiate(bundleGameObject);
				go.layer = LayerMask.NameToLayer(LayerNames.UI);
				UI ui = ComponentFactory.Create<UI, GameObject>(go);

				ui.AddComponent<UILoadingComponent>();
				return ui;
	        }
	        catch (Exception e)
	        {
				Log.Error(e.ToString());
		        return null;
	        }
		}

	    public void Remove(int type)
	    {
	    }
    }
}