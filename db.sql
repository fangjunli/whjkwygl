-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               5.5.25a - MySQL Community Server (GPL)
-- Server OS:                    Win64
-- HeidiSQL version:             7.0.0.4053
-- Date/time:                    2012-08-29 03:14:47
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET FOREIGN_KEY_CHECKS=0 */;

-- Dumping structure for table whecodevarea.t_fee_payed_mgt
DROP TABLE IF EXISTS `t_fee_payed_mgt`;
CREATE TABLE IF NOT EXISTS `t_fee_payed_mgt` (
  `feepayedid` int(11) NOT NULL AUTO_INCREMENT,
  `contractid` int(11) DEFAULT NULL,
  `ppid` int(11) DEFAULT NULL,
  `feemonth` varchar(8) DEFAULT NULL,
  `payedfee` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`feepayedid`)
) ENGINE=InnoDB DEFAULT CHARSET=gb2312 ROW_FORMAT=COMPACT;

-- Data exporting was unselected.


-- Dumping structure for table whecodevarea.t_fee_payed_seq_con_mgt
DROP TABLE IF EXISTS `t_fee_payed_seq_con_mgt`;
CREATE TABLE IF NOT EXISTS `t_fee_payed_seq_con_mgt` (
  `feepayedctseq` int(11) NOT NULL AUTO_INCREMENT,
  `contractid` int(11) DEFAULT NULL,
  `contractno` varchar(45) DEFAULT NULL,
  `payeddate` varchar(8) DEFAULT NULL,
  `payedfee` decimal(10,2) DEFAULT NULL,
  `isconfirmed` varchar(2) DEFAULT NULL,
  PRIMARY KEY (`feepayedctseq`)
) ENGINE=InnoDB DEFAULT CHARSET=gb2312 ROW_FORMAT=COMPACT;

-- Data exporting was unselected.


-- Dumping structure for table whecodevarea.t_fee_payed_seq_mgt
DROP TABLE IF EXISTS `t_fee_payed_seq_mgt`;
CREATE TABLE IF NOT EXISTS `t_fee_payed_seq_mgt` (
  `seqid` int(11) NOT NULL AUTO_INCREMENT,
  `feepayedctseq` int(11) NOT NULL,
  `contractid` int(11) DEFAULT NULL,
  `ppid` int(11) DEFAULT NULL,
  `feemonth` varchar(8) DEFAULT NULL,
  `payfee` decimal(10,2) DEFAULT NULL,
  `feepayedprev` decimal(10,2) DEFAULT NULL,
  `feepayednow` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`seqid`)
) ENGINE=InnoDB DEFAULT CHARSET=gb2312;

-- Data exporting was unselected.


-- Dumping structure for table whecodevarea.t_fee_pay_mgt
DROP TABLE IF EXISTS `t_fee_pay_mgt`;
CREATE TABLE IF NOT EXISTS `t_fee_pay_mgt` (
  `feeid` int(11) NOT NULL AUTO_INCREMENT,
  `contractid` int(11) DEFAULT NULL,
  `contractno` varchar(45) DEFAULT NULL,
  `ppid` int(11) DEFAULT NULL,
  `unitno` varchar(45) DEFAULT NULL,
  `cusid` varchar(45) DEFAULT NULL,
  `cusno` varchar(45) DEFAULT NULL,
  `feemonth` varchar(8) DEFAULT NULL,
  `rentfee` decimal(10,2) DEFAULT NULL,
  `bfee` decimal(10,2) DEFAULT NULL,
  `feefree` varchar(2) DEFAULT NULL,
  PRIMARY KEY (`feeid`)
) ENGINE=InnoDB DEFAULT CHARSET=gb2312;

-- Data exporting was unselected.
/*!40014 SET FOREIGN_KEY_CHECKS=1 */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
