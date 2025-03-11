using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
         GameObject characterInstance = GameObject.Instantiate(character, Vector3.zero, Quaternion.identity);
        
            Press(keyboard.upArrowKey);
            yield return new WaitForSeconds(1f);
            Release(keyboard.upArrowKey);
            yield return new WaitForSeconds(1f);
        
            Assert.That(characterInstance.transform.GetChild(0).transform.position.z, Is.GreaterThan(1.5f));
    }
}
