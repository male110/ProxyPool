CREATE TABLE `Proxy` (
	`Id` INT(11) NOT NULL AUTO_INCREMENT,
	`Adress` VARCHAR(36) NOT NULL DEFAULT '' COMMENT 'ip地址',
	`Port` INT(11) NOT NULL DEFAULT '80' COMMENT '端口号',
	`Source` VARCHAR(50) NOT NULL DEFAULT '' COMMENT '来源',
	`Speed` INT(11) NOT NULL DEFAULT '0' COMMENT '每秒下载的字节数',
	`CreateDate` DATETIME NOT NULL COMMENT '抓取的日期',
	`LastVerifyDate` DATETIME NOT NULL COMMENT '最后验证日期',
	PRIMARY KEY (`Id`),
	INDEX `AdressPortIndex` (`Adress`, `Port`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
AUTO_INCREMENT=1
;
