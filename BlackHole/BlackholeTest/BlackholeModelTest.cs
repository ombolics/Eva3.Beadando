using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlackHoleGame.Model;
using BlackHoleGame.Persistance;
using System;

namespace BlackholeTest
{
    [TestClass]
    public class BlackholeModelTest
    {
        private BlackHoleModel model;

        [TestInitialize]
        public void Initialize()
        {
            model = new BlackHoleModel();
            model.GameUpdated += (a,b) => { };
            model.GameOver += (a, b) => { };
        }

        [TestMethod]
        public void NewGame5Test()
        {
            model.NewGame(5);
            Assert.AreEqual(5, model.TableSize);
            Assert.AreEqual(0,model.P1InTheHole);
            Assert.AreEqual(0, model.P2InTheHole);
            Assert.AreEqual(true, model.FirstsTurn);
        }

        [TestMethod]
        public void NewGame7Tes()
        {
            model.NewGame(7);
            Assert.AreEqual(7, model.TableSize);
            Assert.AreEqual(0, model.P1InTheHole);
            Assert.AreEqual(0, model.P2InTheHole);
            Assert.AreEqual(true, model.FirstsTurn);

        }

        [TestMethod]
        public void NewGame9Test()
        {
            model.NewGame(9);
            Assert.AreEqual(9, model.TableSize);
            Assert.AreEqual(0, model.P1InTheHole);
            Assert.AreEqual(0, model.P2InTheHole);
            Assert.AreEqual(true, model.FirstsTurn);

        }

        [TestMethod]
        public void SaveGameTest()
        {
            model.NewGame(29);
            Assert.AreEqual(true, model.SaveGame(@"..\..\..\..\TestFiles\test1.txt"));
        }

        [TestMethod]
        public void LoadGameTest()
        {
            model.NewGame(29);
            model.SaveGame(@"..\..\..\..\TestFiles\test29by29.txt");

            Assert.AreEqual(true, model.LoadGame(@"..\..\..\..\TestFiles\test29by29.txt"));
            Assert.AreEqual(29, model.TableSize);
            Assert.AreEqual(0, model.P1InTheHole);
            Assert.AreEqual(0, model.P2InTheHole);
            Assert.AreEqual(true, model.FirstsTurn);
        }

        [TestMethod]
        public void MoveTest1()
        {
            model.NewGame(5);
            model.Move((0, 0), (2, 2), Entity.PLAYER1);
            Assert.AreEqual(1, model.P1InTheHole);
            model.Move((1, 1), (2, 2), Entity.PLAYER1);
            Assert.AreEqual(2, model.P1InTheHole);
            model.Move((0, 4), (2, 2), Entity.PLAYER1);
            Assert.AreEqual(3, model.P1InTheHole);

            model.Move((3, 3), (2, 2), Entity.PLAYER2);
            Assert.AreEqual(1, model.P2InTheHole);
            model.Move((4, 4), (2, 2), Entity.PLAYER2);
            Assert.AreEqual(2, model.P2InTheHole);
        }

        [TestMethod]
        public void moveTest2()
        {
            model.NewGame(7);
            model.Move((0, 0), (3, 3), Entity.PLAYER1);
            Assert.AreEqual(1, model.P1InTheHole);
            model.Move((1, 1), (3, 3), Entity.PLAYER1);
            Assert.AreEqual(2, model.P1InTheHole);

            model.Move((4, 4), (3, 3), Entity.PLAYER2);
            Assert.AreEqual(1, model.P2InTheHole);
            model.Move((6, 6), (3, 3), Entity.PLAYER2);
            Assert.AreEqual(2, model.P2InTheHole);
        }

        [TestMethod]
        public void moveTest3()
        {
            model.LoadGame(@"..\..\..\..\TestFiles\test1.txt");
            model.Move((0, 0), (14, 14), Entity.PLAYER1);
            Assert.AreEqual(1, model.P1InTheHole);
            model.Move((1, 1), (14, 14), Entity.PLAYER1);
            Assert.AreEqual(2, model.P1InTheHole);

            model.Move((16, 16), (14, 14), Entity.PLAYER2);
            Assert.AreEqual(1, model.P2InTheHole);
            model.Move((20, 20), (14, 14), Entity.PLAYER2);
            Assert.AreEqual(2, model.P2InTheHole);
        }
    }
}
