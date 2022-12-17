using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RecycleCoinMvc.Models;
using System.Collections.Generic;

namespace RecycleCoin.Test
{
    [TestClass]
    public class UnitTest
    {

        [TestMethod]
        public void IsConnectionToBlockchainApi()
        {
            //Act
            var blockchainApi = new BlockchainApi();
            //Arange
            string defaultString = blockchainApi.Get("/").ToString();

            //Assert
            Assert.AreEqual(defaultString, "Hello World!");
        }

        [TestMethod]
        public void AfterChangingTheBlockchainSettingsIsItTheSameAsBeforeItChanged()
        {
            //Act
            var blockchainApi = new BlockchainApi();

            //Arange
            var settings = blockchainApi.Get("api/Blockchain/getDifficultyAndminingReward");
            int difficulty = settings["difficulty"];
            int miningReward = settings["miningReward"]; // Mevcut Ayarları çeker

            var dif = new JProperty("difficulty", difficulty + 1); // mevcut zorluk seviyesi (difficulty) ayarını 1 arttırır
            var rew = new JProperty("reward", miningReward + 20); // mevcut kazı ödülünü (miningreward) 20 arttırır
            var newSettings = blockchainApi.Post("api/Blockchain/setDifficultyAndminingReward", new List<JProperty> { dif, rew }); // sunucuya yeni ayarların güncellenmesi için istekte bulunur 

            //Assert
            Assert.AreNotEqual(newSettings, settings); // Mevcut ve yeni ayarlar aynı olmamalı 


        }

        [TestMethod]
        public void GenerateKeyPairForNewUser()
        {
            //Act
            var blockchainApi = new BlockchainApi();
            
            //Arange
            var jRes = blockchainApi.Get("api/User/generateKeyPair");
            var publickey = jRes["publicKey"];
            var privateKey = jRes["privateKey"];
            
            //Assert
            Assert.IsNotNull(privateKey);
            Assert.IsNotNull(publickey);
        }
    }
}
