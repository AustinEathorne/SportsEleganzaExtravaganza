using UnityEditor;
using UnityEngine;
using System;

[Flags]		// Bitwise Flags use the individual digits of a number to represent a sequence of boolean values
public enum EditorListOptions		// Prevents the function call for Show() from becoming too obscure
{
	None = 0,
	ListSize = 1,
	ListLabel = 2,
	ElementLabels = 4,		// When using bitwise, we need to skip 3 (listSize + listLabels)
	Buttons = 8,			// Will allow us to duplicate, delete, and move from our lists
	Default = ListSize | ListLabel | ElementLabels,
	NoElementLabels = ListSize | ListLabel,
	All = Default | Buttons
}


public static class EditorList
{
	public static void Show (SerializedProperty list, EditorListOptions options = EditorListOptions.Default)
	{
		EditorGUI.indentLevel = 1;

		bool
			showListLabel = (options & EditorListOptions.ListLabel) != 0,		// bitwise operations
			showListSize = (options & EditorListOptions.ListSize) != 0;			

		if(showListLabel)
		{
			EditorGUILayout.PropertyField (list);
			EditorGUI.indentLevel += 1; 	// add indent level before showing elements
		}

		if(!showListLabel || list.isExpanded)		// toggle for collapsed/expanded foldout in list
		{
			SerializedProperty size = list.FindPropertyRelative ("Array.size");		// If editing multiple lists of different sizes, we want to unShow the list's elements to avoid errors
			if(showListSize)
			{
				EditorGUILayout.PropertyField (size);
			}

			if(size.hasMultipleDifferentValues)
			{
				EditorGUILayout.HelpBox ("Not showing lists with different sizes.", MessageType.Info);
			}
			else
			{
				ShowElements (list, options);
			}
		}

		if(showListLabel)
		{
			EditorGUI.indentLevel -= 1; 	// reduce indent level after showing elements
		}
	}


	private static GUIContent
		moveButtonContent = new GUIContent("\u21b4", "Move Down"),		// u21b4 is a rightwards arrow with corner pointing downwards (opposite of enter button)
		duplicateButtonContent = new GUIContent("+", "Duplicate"),
		deleteButtonContent = new GUIContent("-", "Delete"),
		addButtonContent = new GUIContent("+", "Add Element");


	// Extracts the option to hide the labels of the elements if necessary
	private static void ShowElements(SerializedProperty list, EditorListOptions options)
	{
		if(!list.isArray)
		{
			EditorGUILayout.HelpBox (list.name + " is neither an array nor a list!", MessageType.Error);
			return;
		}
		bool 
			showElementLabels = (options & EditorListOptions.ElementLabels) != 0,
			showButtons = (options & EditorListOptions.Buttons) != 0;

		for(int i = 0; i <list.arraySize; i++)
		{
			if(showButtons)
			{
				EditorGUILayout.BeginHorizontal ();		// Forces buttons to stay on a single line, rather than vertically aligned
			}

			if(showElementLabels)
			{
				EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));	// Show each property at each index
			}
			else
			{
				EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), GUIContent.none);
			}

			if(showButtons)
			{
				ShowButtons (list, i);
				EditorGUILayout.EndHorizontal ();		// ends the single line started above
			}
		}
		if(showButtons && list.arraySize == 0 && GUILayout.Button(addButtonContent, EditorStyles.miniButton))
		{
			list.arraySize += 1;		// Allows us to easily add a new array element
		}
	}


	private static GUILayoutOption miniButtonWidth = GUILayout.Width(20.0f);

	private static void ShowButtons(SerializedProperty list, int index)
	{
		if(GUILayout.Button (moveButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
		{
			list.MoveArrayElement (index, index + 1);		// moves element
		}
		if(GUILayout.Button (duplicateButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
		{
			list.InsertArrayElementAtIndex (index);			// inserts element
		}
		if(GUILayout.Button (deleteButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
		{
			int oldSize = list.arraySize;
			list.DeleteArrayElementAtIndex (index);
			if(list.arraySize == oldSize)
			{
				list.DeleteArrayElementAtIndex (index);			// deletes element, protects against nullRef
			}
		}
	}

}
