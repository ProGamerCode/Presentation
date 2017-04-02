﻿using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.Presentation 
{
	/// <summary>
	/// Presentation asset.
	/// </summary>
	[CreateAssetMenu(fileName = "Slide Deck", menuName = "Slide Deck")]
	public class SlideDeck : ScriptableObject 
	{

		public enum PlayModeType
		{
			PlayMode				= 1 << 0,
			NonPlayMode				= 1 << 1,
			All						= PlayMode | NonPlayMode
		}

		public enum VisibilityType
		{
			Visible 				= 1 << 0,
			Invisible 				= 1 << 1,
			All						= Visible | Invisible
		}

		public List<PresentationSlide> Slides = new List<PresentationSlide>();

		public bool IsSavedOnDisk
		{
			get
			{
#if UNITY_EDITOR
				return !string.IsNullOrEmpty(Path);
#else
				return true;
#endif
			}
		}

		public string Path
		{
			get
			{
#if UNITY_EDITOR
				return AssetDatabase.GetAssetPath(this);
#else
				return null;
#endif
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

#if UNITY_EDITOR
		public void Save(bool createAssetIfNeeded = false)
		{
			if (IsSavedOnDisk)
			{
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssets();
			}
			else
			{
				if (createAssetIfNeeded)
				{
					var path = EditorUtility.SaveFilePanelInProject("Save Slide Deck", "Presentation.asset", "asset", "");
					if (string.IsNullOrEmpty(path)) return;

					this.hideFlags = HideFlags.None;
					AssetDatabase.CreateAsset(this, path);
					AssetDatabase.SaveAssets();
				}
			}
		}

		public void BakeSlides()
		{
			foreach (var slide in Slides)
			{
				slide.Bake();
			}
		}
#endif

		public List<PresentationSlide> GetSlides(PlayModeType playmode, VisibilityType visibility)
		{
			if (playmode == PlayModeType.All && visibility == VisibilityType.All) return new List<PresentationSlide>(Slides);

			var list = new List<PresentationSlide>();

			foreach (var slide in Slides)
			{
				if (slide.StartInPlayMode)
				{
					if ((playmode & PlayModeType.PlayMode) == 0) continue;
				}
				else
				{
					if ((playmode & PlayModeType.NonPlayMode) == 0) continue;
				}

				if (slide.Visible)
				{
					if ((visibility & VisibilityType.Visible) == 0) continue;
				}
				else
				{
					if ((visibility & VisibilityType.Invisible) == 0) continue;
				}

				list.Add(slide);
			}

			return list;
		}

	}

	/// <summary>
	/// Presentation slide.
	/// </summary>
	[Serializable]
	public class PresentationSlide
	{
		public string ScenePath
		{
			get 
			{
#if UNITY_EDITOR
				return AssetDatabase.GetAssetPath(Scene);
#else
				return scenePath;
#endif
			}
		}

#if UNITY_EDITOR
		public void Bake()
		{
			scenePath = ScenePath;
		}

		public SceneAsset Scene;
#endif
#pragma warning disable 414
		[SerializeField]
		private string scenePath;
#pragma warning restore 414
		public bool Visible = true;
		public bool StartInPlayMode = true;
	}
}