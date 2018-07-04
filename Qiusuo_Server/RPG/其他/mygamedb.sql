-- MySQL dump 10.13  Distrib 5.7.12, for Win64 (x86_64)
--
-- Host: localhost    Database: mygamedb
-- ------------------------------------------------------
-- Server version	5.7.17-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `goods`
--

DROP TABLE IF EXISTS `goods`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `goods` (
  `GoodsId` int(11) NOT NULL AUTO_INCREMENT COMMENT '物品ID',
  `GoodsName` varchar(45) NOT NULL DEFAULT '未知物品名字' COMMENT '物品名字',
  `GoodsLeve` int(11) NOT NULL DEFAULT '1' COMMENT '物品等级',
  `GoodsType` varchar(45) NOT NULL DEFAULT '未知物品类型' COMMENT '物品类型,装备武器药品任务',
  `GoodsDiscript` varchar(45) NOT NULL DEFAULT '未知物品详情' COMMENT '物品描述',
  PRIMARY KEY (`GoodsId`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COMMENT='所有物品信息';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `goods`
--

LOCK TABLES `goods` WRITE;
/*!40000 ALTER TABLE `goods` DISABLE KEYS */;
INSERT INTO `goods` VALUES (1,'续命丹',1,'药品','起死回生'),(2,'内力丹',1,'药品','内力大增'),(3,'倚天剑',1,'武器','倚天不出,谁与争锋'),(4,'屠龙刀',1,'武器','宝刀屠龙,称霸武林'),(5,'金缕玉衣',1,'装备','传说中的华服'),(6,'伊利丹的头颅',1,'任务','任务'),(7,'***',1,'未知物品类型','未知物品详情');
/*!40000 ALTER TABLE `goods` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inventory`
--

DROP TABLE IF EXISTS `inventory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `inventory` (
  `InventoryId` int(11) NOT NULL AUTO_INCREMENT COMMENT '储物清单ID',
  `InventoryRoleId` int(11) NOT NULL COMMENT '财产属于哪个角色',
  `InventoryGoodsId` int(11) NOT NULL COMMENT '财产属于哪个物品',
  `inventoryCount` int(11) DEFAULT '0',
  PRIMARY KEY (`InventoryId`),
  KEY `FkInventoryRole_idx` (`InventoryRoleId`),
  KEY `FkInventoryGoods_idx` (`InventoryGoodsId`),
  CONSTRAINT `FkInventoryGoods` FOREIGN KEY (`InventoryGoodsId`) REFERENCES `goods` (`GoodsId`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FkInventoryRole` FOREIGN KEY (`InventoryRoleId`) REFERENCES `role` (`RoleId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COMMENT='财产清单';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inventory`
--

LOCK TABLES `inventory` WRITE;
/*!40000 ALTER TABLE `inventory` DISABLE KEYS */;
INSERT INTO `inventory` VALUES (2,1,5,1);
/*!40000 ALTER TABLE `inventory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `role`
--

DROP TABLE IF EXISTS `role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `role` (
  `RoleId` int(11) NOT NULL AUTO_INCREMENT COMMENT '角色ID',
  `RoleName` varchar(45) NOT NULL DEFAULT '没有名字' COMMENT '角色名字',
  `RoleSex` varchar(45) NOT NULL DEFAULT '没有性别' COMMENT '角色性别',
  `RoleLevel` int(11) NOT NULL DEFAULT '1' COMMENT '角色等级',
  `RoleType` varchar(45) NOT NULL DEFAULT '战士' COMMENT '角色类型',
  `RoleUserId` int(11) NOT NULL COMMENT '角色所属账号',
  `RolePassword` varchar(45) DEFAULT NULL,
  `RoleRegisterDate` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`RoleId`),
  UNIQUE KEY `RoleName_UNIQUE` (`RoleName`),
  KEY `FkRoleUser_idx` (`RoleUserId`),
  CONSTRAINT `FkRoleUser` FOREIGN KEY (`RoleUserId`) REFERENCES `user` (`UserId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COMMENT='所有角色信息';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `role`
--

LOCK TABLES `role` WRITE;
/*!40000 ALTER TABLE `role` DISABLE KEYS */;
INSERT INTO `role` VALUES (1,'东方不败','女',1,'战士',1,NULL,''),(4,'石满','男',1,'战士',80,NULL,'02/22/2018 20:49:15');
/*!40000 ALTER TABLE `role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `user` (
  `UserId` int(11) NOT NULL AUTO_INCREMENT COMMENT '用户ID',
  `UserName` varchar(45) NOT NULL COMMENT '用户名',
  `UserPassword` varchar(45) NOT NULL DEFAULT '00000000' COMMENT '用户密码',
  `UserPhone` varchar(45) DEFAULT '没有手机号码',
  `UserIdCard` varchar(45) DEFAULT '没有身份证',
  `UserRegisterDate` varchar(45) NOT NULL DEFAULT '注册日期不详' COMMENT '用户注册日期',
  PRIMARY KEY (`UserId`),
  UNIQUE KEY `UsersName_UNIQUE` (`UserName`)
) ENGINE=InnoDB AUTO_INCREMENT=81 DEFAULT CHARSET=utf8 COMMENT='所有账号信息';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,'1','1',NULL,NULL,'2017-12-23 11:09:41'),(80,'2','2','2',NULL,'02/22/2018 20:49:04');
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-02-22 20:51:06
