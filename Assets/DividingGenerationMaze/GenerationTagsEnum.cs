using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerationTagsEnum : PropertyAttribute {

	public static string[] items=new string[]{"Wall", "Corridor", "None", "Junction", "J3", "J4", "Floor", "Cabinet", "Lamp", "Column"};
	public static string[] TagsByDefault=items;
	public int selected = 0;

	//public GenerationTagsEnum(string[] enumerations){ items = enumerations; }

	public GenerationTagsEnum(){
	}


	public static void AddTag(string tag)
	{

		string[] newTags=new string[items.Length+1];

		for (int i = 0; i < items.Length; i++) {
			newTags [i] = items [i];

		}
		newTags [newTags.Length-1] = tag;

		items = newTags;

	}
//	public GenerationTagsEnum(List<string> enumerations){
//		this.items = enumerations.ToArray ();
//	}

}


