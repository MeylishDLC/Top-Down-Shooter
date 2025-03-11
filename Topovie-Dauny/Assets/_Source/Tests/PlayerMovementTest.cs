using System.Collections;
using System.Text.RegularExpressions;
using Core.InputSystem;
using DialogueSystem;
using NUnit.Framework;
using Player.PlayerControl;
using UI.UIShop;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlayerMovementTest : InputTestFixture
    {
        private PlayerMovement _player;
        private Keyboard _keyboard;

        public override void Setup()
        {
            SceneManager.LoadScene("TEST");
            
            // Disabling logs because of annoying dotween package error I literally cant suppress any other way((
            Debug.unityLogger.logEnabled = false;

            base.Setup();
            
            _keyboard = InputSystem.AddDevice<Keyboard>();
        }
        
        [UnityTest]
        public IEnumerator TestPlayerMoveUp()
        {
            yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "TEST");

            _player = Object.FindFirstObjectByType<PlayerMovement>();
            var playerInitPos = _player.transform.position.y;
            Press(_keyboard.upArrowKey);
            yield return new WaitForSeconds(1f);
            Release(_keyboard.upArrowKey);
            yield return new WaitForSeconds(1f);
            
            Assert.That(playerInitPos, Is.LessThan(_player.transform.position.y));
        }
        
        [UnityTest]
        public IEnumerator TestPlayerMoveDown()
        {
            yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "TEST");

            _player = Object.FindFirstObjectByType<PlayerMovement>();
            var playerInitPos = _player.transform.position.y;
            Press(_keyboard.downArrowKey);
            yield return new WaitForSeconds(1f);
            Release(_keyboard.downArrowKey);
            yield return new WaitForSeconds(1f);
            
            Assert.That(playerInitPos, Is.GreaterThan(_player.transform.position.y));
        }
        
        [UnityTest]
        public IEnumerator TestPlayerMoveLeft()
        {
            yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "TEST");
            
            _player = Object.FindFirstObjectByType<PlayerMovement>();
            var playerInitPos = _player.transform.position.x;
            Press(_keyboard.leftArrowKey);
            yield return new WaitForSeconds(1f);
            Release(_keyboard.leftArrowKey);
            yield return new WaitForSeconds(1f);
            
            Assert.That(playerInitPos, Is.GreaterThan(_player.transform.position.x));
        }
        
        [UnityTest]
        public IEnumerator TestPlayerMoveRight()
        {
            yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "TEST");

            _player = Object.FindFirstObjectByType<PlayerMovement>();
            var playerInitPos = _player.transform.position.x;
            Press(_keyboard.rightArrowKey);
            yield return new WaitForSeconds(1f);
            Release(_keyboard.rightArrowKey);
            yield return new WaitForSeconds(1f);
            
            Assert.That(playerInitPos, Is.LessThan(_player.transform.position.x));
        }

        public override void TearDown()
        {
            base.TearDown();
            Debug.unityLogger.logEnabled = true;
        }
    }
}
