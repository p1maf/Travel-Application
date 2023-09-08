-- MySqlBackup.NET 2.3
-- Dump Time: 2023-06-14 09:12:52
-- --------------------------------------
-- Server version 8.0.30 MySQL Community Server - GPL


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- 
-- Definition of air_travel
-- 

DROP TABLE IF EXISTS `air_travel`;
CREATE TABLE IF NOT EXISTS `air_travel` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table air_travel
-- 
-- Definition of city
-- 

DROP TABLE IF EXISTS `city`;
CREATE TABLE IF NOT EXISTS `city` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` text NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1118 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table city
-- 

-- 
-- Definition of conveniences
-- 

DROP TABLE IF EXISTS `conveniences`;
CREATE TABLE IF NOT EXISTS `conveniences` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` text NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table conveniences
-- 
-- Definition of countries
-- 

DROP TABLE IF EXISTS `countries`;
CREATE TABLE IF NOT EXISTS `countries` (
  `id` varchar(64) NOT NULL,
  `name` text NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table countries
-- 
-- Definition of hotel
-- 

DROP TABLE IF EXISTS `hotel`;
CREATE TABLE IF NOT EXISTS `hotel` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` text NOT NULL,
  `type_food` int NOT NULL,
  `type_room` json NOT NULL,
  `star` int NOT NULL,
  `conveniences` json DEFAULT NULL,
  `country` varchar(45) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `foodid_idx` (`type_food`),
  KEY `idcountryhotel_idx` (`country`),
  CONSTRAINT `foodid` FOREIGN KEY (`type_food`) REFERENCES `type_food` (`id`),
  CONSTRAINT `idcountryhotel` FOREIGN KEY (`country`) REFERENCES `countries` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table hotel
-- Definition of role
-- 

DROP TABLE IF EXISTS `role`;
CREATE TABLE IF NOT EXISTS `role` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(20) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table role
-- 

/*!40000 ALTER TABLE `role` DISABLE KEYS */;
INSERT INTO `role`(`id`,`name`) VALUES
(1,'Администратор'),
(2,'Менеджер');
/*!40000 ALTER TABLE `role` ENABLE KEYS */;

-- 
-- Definition of status_tour
-- 

DROP TABLE IF EXISTS `status_tour`;
CREATE TABLE IF NOT EXISTS `status_tour` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(45) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='	';

-- 
-- Dumping data for table status_tour
-- 
-- Definition of tour_operator
-- 

DROP TABLE IF EXISTS `tour_operator`;
CREATE TABLE IF NOT EXISTS `tour_operator` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` text NOT NULL,
  `legal_name` text NOT NULL,
  `INN` text NOT NULL,
  `OGRN` text NOT NULL,
  `address` text NOT NULL,
  `contract` int NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table tour_operator
-- 
-- 
-- Definition of tourist
-- 

DROP TABLE IF EXISTS `tourist`;
CREATE TABLE IF NOT EXISTS `tourist` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(20) NOT NULL,
  `surname` varchar(25) NOT NULL,
  `patronymic` varchar(25) DEFAULT NULL,
  `birthday` date NOT NULL,
  `serial_passport` varchar(4) NOT NULL,
  `number_passport` varchar(6) NOT NULL,
  `whom_issued` text NOT NULL,
  `number_phone` varchar(12) NOT NULL,
  `address` text NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table tourist
-- 
-- Definition of type_food
-- 

DROP TABLE IF EXISTS `type_food`;
CREATE TABLE IF NOT EXISTS `type_food` (
  `id` int NOT NULL,
  `name` varchar(45) NOT NULL,
  `description` text NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table type_food
-- 
-- 
-- Definition of type_payment
-- 

DROP TABLE IF EXISTS `type_payment`;
CREATE TABLE IF NOT EXISTS `type_payment` (
  `id` int NOT NULL,
  `name` varchar(20) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table type_payment
--  of payment
-- 

DROP TABLE IF EXISTS `payment`;
CREATE TABLE IF NOT EXISTS `payment` (
  `id` int NOT NULL AUTO_INCREMENT,
  `date` date NOT NULL,
  `amount` int NOT NULL,
  `remains` int NOT NULL,
  `type` int NOT NULL,
  `id_travel` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idtypepayment_idx` (`type`),
  KEY `idtravel_idx` (`id_travel`),
  CONSTRAINT `idtravel` FOREIGN KEY (`id_travel`) REFERENCES `travel_package` (`id`),
  CONSTRAINT `idtypepayment` FOREIGN KEY (`type`) REFERENCES `type_payment` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table payment
-- ion of type_room
-- 

DROP TABLE IF EXISTS `type_room`;
CREATE TABLE IF NOT EXISTS `type_room` (
  `id` int NOT NULL,
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table type_room
-- 
-- Definition of tour
-- 

DROP TABLE IF EXISTS `tour`;
CREATE TABLE IF NOT EXISTS `tour` (
  `id` int NOT NULL AUTO_INCREMENT,
  `departure_city` int NOT NULL,
  `country` varchar(45) NOT NULL,
  `start_date` date NOT NULL,
  `end_date` date NOT NULL,
  `quantity_night` int NOT NULL,
  `hotel` int NOT NULL,
  `type_food` int NOT NULL,
  `type_room` int NOT NULL,
  `placeInRoom` int NOT NULL,
  `air_travel` int NOT NULL,
  `tour_operator` int NOT NULL,
  `price` int NOT NULL,
  `status` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idtypefood_idx` (`type_food`),
  KEY `idtyperoom_idx` (`type_room`),
  KEY `idtouroperator_idx` (`tour_operator`),
  KEY `idairtravel_idx` (`air_travel`),
  KEY `idstatustour_idx` (`status`),
  KEY `idhotel_idx` (`hotel`),
  KEY `iddeparture_city_idx` (`departure_city`),
  KEY `idcountry_idx` (`country`),
  CONSTRAINT `idairtravel` FOREIGN KEY (`air_travel`) REFERENCES `air_travel` (`id`),
  CONSTRAINT `idcountry` FOREIGN KEY (`country`) REFERENCES `countries` (`id`),
  CONSTRAINT `iddeparture_city` FOREIGN KEY (`departure_city`) REFERENCES `city` (`id`),
  CONSTRAINT `idhotel` FOREIGN KEY (`hotel`) REFERENCES `hotel` (`id`),
  CONSTRAINT `idstatustour` FOREIGN KEY (`status`) REFERENCES `status_tour` (`id`),
  CONSTRAINT `idtouroperator` FOREIGN KEY (`tour_operator`) REFERENCES `tour_operator` (`id`),
  CONSTRAINT `idtypefood` FOREIGN KEY (`type_food`) REFERENCES `type_food` (`id`),
  CONSTRAINT `idtyperoom` FOREIGN KEY (`type_room`) REFERENCES `type_room` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=131 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table tour
-- 
-- 
-- Definition of travel_package
-- 

DROP TABLE IF EXISTS `travel_package`;
CREATE TABLE IF NOT EXISTS `travel_package` (
  `id` int NOT NULL AUTO_INCREMENT,
  `reg_date` date NOT NULL,
  `idtour` int NOT NULL,
  `idpayer` int NOT NULL,
  `idmanager` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idtour_idx` (`idtour`),
  KEY `idpayer_idx` (`idpayer`),
  KEY `idmanager_idx` (`idmanager`),
  CONSTRAINT `idmanagerr` FOREIGN KEY (`idmanager`) REFERENCES `users` (`id`),
  CONSTRAINT `idpayer` FOREIGN KEY (`idpayer`) REFERENCES `tourist` (`id`),
  CONSTRAINT `idtour` FOREIGN KEY (`idtour`) REFERENCES `tour` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table travel_package
-- 

-- Definition of tourist_travel
-- 

DROP TABLE IF EXISTS `tourist_travel`;
CREATE TABLE IF NOT EXISTS `tourist_travel` (
  `id` int NOT NULL AUTO_INCREMENT,
  `idtravel` int NOT NULL,
  `idtourist` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idtourist_idx` (`idtourist`),
  KEY `idtravel_package_idx` (`idtravel`),
  CONSTRAINT `idtourist_travel` FOREIGN KEY (`idtourist`) REFERENCES `tourist` (`id`),
  CONSTRAINT `idtravel_package` FOREIGN KEY (`idtravel`) REFERENCES `travel_package` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table tourist_travel
-- 

-- Definition of users
-- 

DROP TABLE IF EXISTS `users`;
CREATE TABLE IF NOT EXISTS `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(20) NOT NULL,
  `surname` varchar(20) NOT NULL,
  `patronymic` varchar(25) DEFAULT NULL,
  `login` varchar(12) NOT NULL,
  `password` text NOT NULL,
  `role` int NOT NULL,
  `delUser` tinyint NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idrole_idx` (`role`),
  CONSTRAINT `idrole` FOREIGN KEY (`role`) REFERENCES `role` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table users
-- 

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;


-- Dump completed on 2023-06-14 09:12:52
-- Total time: 0:0:0:0:80 (d:h:m:s:ms)
