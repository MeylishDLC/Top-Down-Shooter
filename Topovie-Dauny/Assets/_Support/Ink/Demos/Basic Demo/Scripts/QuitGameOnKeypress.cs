using UnityEngine;

namespace _Support.Ink.Demos.Basic_Demo.Scripts
{
	public class QuitGameOnKeypress : MonoBehaviour {
	
		public KeyCode key = KeyCode.Escape;
	
		void Update () {
			if(Input.GetKeyDown(key)) Application.Quit();
		}
	}
}