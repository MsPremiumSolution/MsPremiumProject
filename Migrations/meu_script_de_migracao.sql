CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `pais` (
    `PaisID` bigint unsigned NOT NULL AUTO_INCREMENT,
    `NomePais` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    CONSTRAINT `PK_pais` PRIMARY KEY (`PaisID`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Role` (
    `RoleID` bigint unsigned NOT NULL AUTO_INCREMENT,
    `Nome` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Descricao` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    CONSTRAINT `PK_Role` PRIMARY KEY (`RoleID`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Tipojanelas` (
    `TipoJanelaId` bigint unsigned NOT NULL AUTO_INCREMENT,
    `TipoJanela1` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Descricao` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    CONSTRAINT `PK_Tipojanelas` PRIMARY KEY (`TipoJanelaId`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Tipoobras` (
    `TipoObraId` bigint unsigned NOT NULL AUTO_INCREMENT,
    `Descricao` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Observacoes` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    CONSTRAINT `PK_Tipoobras` PRIMARY KEY (`TipoObraId`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Tipotratamentos` (
    `TipoTratamentoId` bigint unsigned NOT NULL AUTO_INCREMENT,
    `TipoTratamentoNome` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    CONSTRAINT `PK_Tipotratamentos` PRIMARY KEY (`TipoTratamentoId`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Volumes` (
    `VolumeId` bigint unsigned NOT NULL AUTO_INCREMENT,
    `VolumeTotal` decimal(65,30) NOT NULL,
    `SuperficieTotal` decimal(65,30) NOT NULL,
    CONSTRAINT `PK_Volumes` PRIMARY KEY (`VolumeId`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Localidades` (
    `LocalidadeId` bigint unsigned NOT NULL AUTO_INCREMENT,
    `PaisId` bigint unsigned NOT NULL,
    `NomeLocalidade` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Regiao` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    CONSTRAINT `PK_Localidades` PRIMARY KEY (`LocalidadeId`),
    CONSTRAINT `FK_Localidades_pais_PaisId` FOREIGN KEY (`PaisId`) REFERENCES `pais` (`PaisID`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Utilizador` (
    `UtilizadorID` bigint unsigned NOT NULL AUTO_INCREMENT,
    `RoleID` bigint unsigned NOT NULL,
    `Nome` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Login` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `PWP` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Dtnascimento` datetime(6) NOT NULL,
    `Activo` tinyint(1) NOT NULL,
    CONSTRAINT `PK_Utilizador` PRIMARY KEY (`UtilizadorID`),
    CONSTRAINT `FK_Utilizador_Role_RoleID` FOREIGN KEY (`RoleID`) REFERENCES `Role` (`RoleID`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Calculovolumes` (
    `CalculoVolumeId` bigint unsigned NOT NULL AUTO_INCREMENT,
    `VolumeId` bigint unsigned NOT NULL,
    `AlturaMetros` decimal(65,30) NOT NULL,
    `EstadiaDireta` decimal(65,30) NOT NULL,
    `Estadia1` decimal(65,30) NOT NULL,
    `Largura` decimal(65,30) NOT NULL,
    `Comprimento` decimal(65,30) NOT NULL,
    CONSTRAINT `PK_Calculovolumes` PRIMARY KEY (`CalculoVolumeId`),
    CONSTRAINT `FK_Calculovolumes_Volumes_VolumeId` FOREIGN KEY (`VolumeId`) REFERENCES `Volumes` (`VolumeId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Clientes` (
    `ClienteId` bigint unsigned NOT NULL AUTO_INCREMENT,
    `Nome` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Apelido` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Morada` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Cp4` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Cp3` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `LocalidadeId` bigint unsigned NOT NULL,
    `Localidade` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `NumeroFiscal` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Observacoes` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Telefone1` bigint NOT NULL,
    `Telefone2` bigint NOT NULL,
    `Dtnascimento` date NOT NULL,
    CONSTRAINT `PK_Clientes` PRIMARY KEY (`ClienteId`),
    CONSTRAINT `FK_Clientes_Localidades_LocalidadeId` FOREIGN KEY (`LocalidadeId`) REFERENCES `Localidades` (`LocalidadeId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `PasswordResetToken` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UtilizadorId` bigint unsigned NOT NULL,
    `TokenValue` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `ExpirationDate` datetime(6) NOT NULL,
    `IsUsed` tinyint(1) NOT NULL,
    CONSTRAINT `PK_PasswordResetToken` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_PasswordResetToken_Utilizador_UtilizadorId` FOREIGN KEY (`UtilizadorId`) REFERENCES `Utilizador` (`UtilizadorID`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Proposta` (
    `PropostaId` bigint unsigned NOT NULL AUTO_INCREMENT,
    `ClienteId` bigint unsigned NOT NULL,
    `DataProposta` datetime(6) NOT NULL,
    `DataAceitacao` datetime(6) NOT NULL,
    `CodigoProposta` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `UtilizadorId` bigint unsigned NOT NULL,
    `Estado` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `ValorObra` decimal(65,30) NOT NULL,
    CONSTRAINT `PK_Proposta` PRIMARY KEY (`PropostaId`),
    CONSTRAINT `FK_Proposta_Clientes_ClienteId` FOREIGN KEY (`ClienteId`) REFERENCES `Clientes` (`ClienteId`) ON DELETE CASCADE,
    CONSTRAINT `FK_Proposta_Utilizador_UtilizadorId` FOREIGN KEY (`UtilizadorId`) REFERENCES `Utilizador` (`UtilizadorID`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Construtivos` (
    `ConstrutivoId` bigint unsigned NOT NULL AUTO_INCREMENT,
    `PropostaId` bigint unsigned NOT NULL,
    `Data` datetime(6) NOT NULL,
    `AnoConstrucao` decimal(65,30) NOT NULL,
    `Area` decimal(65,30) NOT NULL,
    `Nandares` decimal(65,30) NOT NULL,
    `Nhabitantes` decimal(65,30) NOT NULL,
    `Localidade` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Altitude` tinyint(1) NOT NULL,
    `FechamentoTipoFachada` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `FechamentoOrientacao` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `FechamentoCobertura` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `FechamentoCoberturaPosterior` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `FechamentoTratHidrofugacao` tinyint(1) NOT NULL,
    CONSTRAINT `PK_Construtivos` PRIMARY KEY (`ConstrutivoId`),
    CONSTRAINT `FK_Construtivos_Proposta_PropostaId` FOREIGN KEY (`PropostaId`) REFERENCES `Proposta` (`PropostaId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Dadosgerals` (
    `DadosGeralId` bigint unsigned NOT NULL AUTO_INCREMENT,
    `PropostaId` bigint unsigned NOT NULL,
    `TipoObraId` bigint unsigned NOT NULL,
    `TipoTratamentoId` bigint unsigned NOT NULL,
    `DgTipoFachada` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `DgOrientacao` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `DgCoberturaFprincipal` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `DgCoberturaFposterior` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `DgTratamentoHidrofugacao` tinyint(1) NOT NULL,
    `DgIsolamentoCamera` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `DgIsolamentoInterno` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `DgIsolamentoAquecimento` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `DgTipoJanelaId` bigint unsigned NOT NULL,
    `DgTipoJanelaMaterial` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `DgTipoJanelaDuplas` tinyint(1) NOT NULL,
    `DgTipoJanelaVidro` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `DgTipoJanelaRpt` tinyint(1) NOT NULL,
    `DgTipoJanelaCaixasPersiana` tinyint(1) NOT NULL,
    `DgTipoJanelaUnidades` decimal(65,30) NOT NULL,
    CONSTRAINT `PK_Dadosgerals` PRIMARY KEY (`DadosGeralId`),
    CONSTRAINT `FK_Dadosgerals_Proposta_PropostaId` FOREIGN KEY (`PropostaId`) REFERENCES `Proposta` (`PropostaId`) ON DELETE CASCADE,
    CONSTRAINT `FK_Dadosgerals_Tipojanelas_DgTipoJanelaId` FOREIGN KEY (`DgTipoJanelaId`) REFERENCES `Tipojanelas` (`TipoJanelaId`) ON DELETE CASCADE,
    CONSTRAINT `FK_Dadosgerals_Tipoobras_TipoObraId` FOREIGN KEY (`TipoObraId`) REFERENCES `Tipoobras` (`TipoObraId`) ON DELETE CASCADE,
    CONSTRAINT `FK_Dadosgerals_Tipotratamentos_TipoTratamentoId` FOREIGN KEY (`TipoTratamentoId`) REFERENCES `Tipotratamentos` (`TipoTratamentoId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Higrometria` (
    `HigrometriaId` bigint unsigned NOT NULL AUTO_INCREMENT,
    `DadosGeralId` bigint unsigned NOT NULL,
    `PropostaId` bigint unsigned NOT NULL,
    `HumidadeExterior` decimal(65,30) NOT NULL,
    `TempExterior` decimal(65,30) NOT NULL,
    `HumidadeInterior` decimal(65,30) NOT NULL,
    `TempInterior` decimal(65,30) NOT NULL,
    `TempParedesInt` decimal(65,30) NOT NULL,
    `TempPontosFrios` decimal(65,30) NOT NULL,
    `PontoOrvalho` decimal(65,30) NOT NULL,
    `NivelCo2` decimal(65,30) NOT NULL,
    `NivelTcov` decimal(65,30) NOT NULL,
    `NivelHcho` decimal(65,30) NOT NULL,
    `DataLogger` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    CONSTRAINT `PK_Higrometria` PRIMARY KEY (`HigrometriaId`),
    CONSTRAINT `FK_Higrometria_Dadosgerals_DadosGeralId` FOREIGN KEY (`DadosGeralId`) REFERENCES `Dadosgerals` (`DadosGeralId`) ON DELETE CASCADE,
    CONSTRAINT `FK_Higrometria_Proposta_PropostaId` FOREIGN KEY (`PropostaId`) REFERENCES `Proposta` (`PropostaId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Qualidadears` (
    `QualidadeArId` bigint unsigned NOT NULL AUTO_INCREMENT,
    `ObjetivoId` bigint unsigned NOT NULL,
    `DadosGeralId` bigint unsigned NOT NULL,
    `VolumeId` bigint unsigned NOT NULL,
    CONSTRAINT `PK_Qualidadears` PRIMARY KEY (`QualidadeArId`),
    CONSTRAINT `FK_Qualidadears_Dadosgerals_DadosGeralId` FOREIGN KEY (`DadosGeralId`) REFERENCES `Dadosgerals` (`DadosGeralId`) ON DELETE CASCADE,
    CONSTRAINT `FK_Qualidadears_Volumes_VolumeId` FOREIGN KEY (`VolumeId`) REFERENCES `Volumes` (`VolumeId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Sintomas` (
    `SintomaId` bigint unsigned NOT NULL AUTO_INCREMENT,
    `DadosGeralId` bigint unsigned NOT NULL,
    `PropostaId` bigint unsigned NOT NULL,
    `Fungos` tinyint(1) NOT NULL,
    `Cheiros` tinyint(1) NOT NULL,
    `MofosRoupasArmario` tinyint(1) NOT NULL,
    `CondensacaoJanelas` tinyint(1) NOT NULL,
    `ExcessoAquecimento` tinyint(1) NOT NULL,
    `Alergias` tinyint(1) NOT NULL,
    `GasRadon` tinyint(1) NOT NULL,
    `Esporos` tinyint(1) NOT NULL,
    CONSTRAINT `PK_Sintomas` PRIMARY KEY (`SintomaId`),
    CONSTRAINT `FK_Sintomas_Dadosgerals_DadosGeralId` FOREIGN KEY (`DadosGeralId`) REFERENCES `Dadosgerals` (`DadosGeralId`) ON DELETE CASCADE,
    CONSTRAINT `FK_Sintomas_Proposta_PropostaId` FOREIGN KEY (`PropostaId`) REFERENCES `Proposta` (`PropostaId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE INDEX `IX_Calculovolumes_VolumeId` ON `Calculovolumes` (`VolumeId`);

CREATE INDEX `IX_Clientes_LocalidadeId` ON `Clientes` (`LocalidadeId`);

CREATE INDEX `IX_Construtivos_PropostaId` ON `Construtivos` (`PropostaId`);

CREATE INDEX `IX_Dadosgerals_DgTipoJanelaId` ON `Dadosgerals` (`DgTipoJanelaId`);

CREATE INDEX `IX_Dadosgerals_PropostaId` ON `Dadosgerals` (`PropostaId`);

CREATE INDEX `IX_Dadosgerals_TipoObraId` ON `Dadosgerals` (`TipoObraId`);

CREATE INDEX `IX_Dadosgerals_TipoTratamentoId` ON `Dadosgerals` (`TipoTratamentoId`);

CREATE INDEX `IX_Higrometria_DadosGeralId` ON `Higrometria` (`DadosGeralId`);

CREATE INDEX `IX_Higrometria_PropostaId` ON `Higrometria` (`PropostaId`);

CREATE INDEX `IX_Localidades_PaisId` ON `Localidades` (`PaisId`);

CREATE UNIQUE INDEX `IX_PasswordResetToken_TokenValue` ON `PasswordResetToken` (`TokenValue`);

CREATE INDEX `IX_PasswordResetToken_UtilizadorId` ON `PasswordResetToken` (`UtilizadorId`);

CREATE INDEX `IX_Proposta_ClienteId` ON `Proposta` (`ClienteId`);

CREATE INDEX `IX_Proposta_UtilizadorId` ON `Proposta` (`UtilizadorId`);

CREATE INDEX `IX_Qualidadears_DadosGeralId` ON `Qualidadears` (`DadosGeralId`);

CREATE INDEX `IX_Qualidadears_VolumeId` ON `Qualidadears` (`VolumeId`);

CREATE INDEX `IX_Sintomas_DadosGeralId` ON `Sintomas` (`DadosGeralId`);

CREATE INDEX `IX_Sintomas_PropostaId` ON `Sintomas` (`PropostaId`);

CREATE INDEX `IX_Utilizador_RoleID` ON `Utilizador` (`RoleID`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250612153721_CreateFullSchemaWithToken', '7.0.11');

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS `POMELO_BEFORE_DROP_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID TINYINT(1);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `Extra` = 'auto_increment'
			AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS `POMELO_AFTER_ADD_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255), IN `COLUMN_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID INT(11);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
			AND `COLUMN_TYPE` LIKE '%int%'
			AND `COLUMN_KEY` = 'PRI';
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL AUTO_INCREMENT;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

ALTER TABLE `Calculovolumes` DROP FOREIGN KEY `FK_Calculovolumes_Volumes_VolumeId`;

ALTER TABLE `Clientes` DROP FOREIGN KEY `FK_Clientes_Localidades_LocalidadeId`;

ALTER TABLE `Construtivos` DROP FOREIGN KEY `FK_Construtivos_Proposta_PropostaId`;

ALTER TABLE `Dadosgerals` DROP FOREIGN KEY `FK_Dadosgerals_Proposta_PropostaId`;

ALTER TABLE `Dadosgerals` DROP FOREIGN KEY `FK_Dadosgerals_Tipojanelas_DgTipoJanelaId`;

ALTER TABLE `Dadosgerals` DROP FOREIGN KEY `FK_Dadosgerals_Tipoobras_TipoObraId`;

ALTER TABLE `Dadosgerals` DROP FOREIGN KEY `FK_Dadosgerals_Tipotratamentos_TipoTratamentoId`;

ALTER TABLE `Higrometria` DROP FOREIGN KEY `FK_Higrometria_Dadosgerals_DadosGeralId`;

ALTER TABLE `Higrometria` DROP FOREIGN KEY `FK_Higrometria_Proposta_PropostaId`;

ALTER TABLE `PasswordResetToken` DROP FOREIGN KEY `FK_PasswordResetToken_Utilizador_UtilizadorId`;

ALTER TABLE `Proposta` DROP FOREIGN KEY `FK_Proposta_Clientes_ClienteId`;

ALTER TABLE `Proposta` DROP FOREIGN KEY `FK_Proposta_Utilizador_UtilizadorId`;

ALTER TABLE `Qualidadears` DROP FOREIGN KEY `FK_Qualidadears_Dadosgerals_DadosGeralId`;

ALTER TABLE `Qualidadears` DROP FOREIGN KEY `FK_Qualidadears_Volumes_VolumeId`;

ALTER TABLE `Sintomas` DROP FOREIGN KEY `FK_Sintomas_Dadosgerals_DadosGeralId`;

ALTER TABLE `Sintomas` DROP FOREIGN KEY `FK_Sintomas_Proposta_PropostaId`;

ALTER TABLE `Utilizador` DROP FOREIGN KEY `FK_Utilizador_Role_RoleID`;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Utilizador');
ALTER TABLE `Utilizador` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Role');
ALTER TABLE `Role` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Proposta');
ALTER TABLE `Proposta` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Higrometria');
ALTER TABLE `Higrometria` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Volumes');
ALTER TABLE `Volumes` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Tipotratamentos');
ALTER TABLE `Tipotratamentos` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Tipoobras');
ALTER TABLE `Tipoobras` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Tipojanelas');
ALTER TABLE `Tipojanelas` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Sintomas');
ALTER TABLE `Sintomas` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Qualidadears');
ALTER TABLE `Qualidadears` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Dadosgerals');
ALTER TABLE `Dadosgerals` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Construtivos');
ALTER TABLE `Construtivos` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Clientes');
ALTER TABLE `Clientes` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Calculovolumes');
ALTER TABLE `Calculovolumes` DROP PRIMARY KEY;

ALTER TABLE `Clientes` DROP COLUMN `Localidade`;

ALTER TABLE `Utilizador` RENAME `utilizador`;

ALTER TABLE `Role` RENAME `role`;

ALTER TABLE `Proposta` RENAME `proposta`;

ALTER TABLE `Higrometria` RENAME `higrometria`;

ALTER TABLE `Volumes` RENAME `volume`;

ALTER TABLE `Tipotratamentos` RENAME `tipotratamento`;

ALTER TABLE `Tipoobras` RENAME `tipoobra`;

ALTER TABLE `Tipojanelas` RENAME `tipojanela`;

ALTER TABLE `Sintomas` RENAME `sintoma`;

ALTER TABLE `Qualidadears` RENAME `qualidadear`;

ALTER TABLE `Dadosgerals` RENAME `dadosgeral`;

ALTER TABLE `Construtivos` RENAME `construtivo`;

ALTER TABLE `Clientes` RENAME `cliente`;

ALTER TABLE `Calculovolumes` RENAME `calculovolume`;

ALTER TABLE `utilizador` RENAME INDEX `IX_Utilizador_RoleID` TO `IX_utilizador_RoleID`;

ALTER TABLE `proposta` RENAME INDEX `IX_Proposta_UtilizadorId` TO `IX_proposta_UtilizadorId`;

ALTER TABLE `proposta` RENAME INDEX `IX_Proposta_ClienteId` TO `IX_proposta_ClienteId`;

ALTER TABLE `PasswordResetToken` RENAME COLUMN `UtilizadorId` TO `UtilizadorID`;

ALTER TABLE `PasswordResetToken` RENAME INDEX `IX_PasswordResetToken_UtilizadorId` TO `IX_PasswordResetToken_UtilizadorID`;

ALTER TABLE `higrometria` RENAME INDEX `IX_Higrometria_PropostaId` TO `IX_higrometria_PropostaId`;

ALTER TABLE `higrometria` RENAME INDEX `IX_Higrometria_DadosGeralId` TO `IX_higrometria_DadosGeralId`;

ALTER TABLE `sintoma` RENAME INDEX `IX_Sintomas_PropostaId` TO `IX_sintoma_PropostaId`;

ALTER TABLE `sintoma` RENAME INDEX `IX_Sintomas_DadosGeralId` TO `IX_sintoma_DadosGeralId`;

ALTER TABLE `qualidadear` RENAME INDEX `IX_Qualidadears_VolumeId` TO `IX_qualidadear_VolumeId`;

ALTER TABLE `qualidadear` RENAME INDEX `IX_Qualidadears_DadosGeralId` TO `IX_qualidadear_DadosGeralId`;

ALTER TABLE `dadosgeral` RENAME INDEX `IX_Dadosgerals_TipoTratamentoId` TO `IX_dadosgeral_TipoTratamentoId`;

ALTER TABLE `dadosgeral` RENAME INDEX `IX_Dadosgerals_TipoObraId` TO `IX_dadosgeral_TipoObraId`;

ALTER TABLE `dadosgeral` RENAME INDEX `IX_Dadosgerals_PropostaId` TO `IX_dadosgeral_PropostaId`;

ALTER TABLE `dadosgeral` RENAME INDEX `IX_Dadosgerals_DgTipoJanelaId` TO `IX_dadosgeral_DgTipoJanelaId`;

ALTER TABLE `construtivo` RENAME INDEX `IX_Construtivos_PropostaId` TO `IX_construtivo_PropostaId`;

ALTER TABLE `cliente` RENAME INDEX `IX_Clientes_LocalidadeId` TO `IX_cliente_LocalidadeId`;

ALTER TABLE `calculovolume` RENAME INDEX `IX_Calculovolumes_VolumeId` TO `IX_calculovolume_VolumeId`;

ALTER TABLE `utilizador` MODIFY COLUMN `Login` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Localidades` MODIFY COLUMN `Regiao` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Localidades` MODIFY COLUMN `NomeLocalidade` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `cliente` MODIFY COLUMN `Telefone2` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `cliente` MODIFY COLUMN `Telefone1` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `cliente` MODIFY COLUMN `Observacoes` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `cliente` MODIFY COLUMN `NumeroFiscal` varchar(9) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `cliente` MODIFY COLUMN `Nome` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `cliente` MODIFY COLUMN `Morada` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `cliente` MODIFY COLUMN `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `cliente` MODIFY COLUMN `Dtnascimento` date NULL;

ALTER TABLE `cliente` MODIFY COLUMN `Cp4` varchar(4) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `cliente` MODIFY COLUMN `Cp3` varchar(3) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `cliente` MODIFY COLUMN `Apelido` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `utilizador` ADD CONSTRAINT `PK_utilizador` PRIMARY KEY (`UtilizadorID`);
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'utilizador', 'UtilizadorID');

ALTER TABLE `role` ADD CONSTRAINT `PK_role` PRIMARY KEY (`RoleID`);
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'role', 'RoleID');

ALTER TABLE `proposta` ADD CONSTRAINT `PK_proposta` PRIMARY KEY (`PropostaId`);
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'proposta', 'PropostaId');

ALTER TABLE `higrometria` ADD CONSTRAINT `PK_higrometria` PRIMARY KEY (`HigrometriaId`);
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'higrometria', 'HigrometriaId');

ALTER TABLE `volume` ADD CONSTRAINT `PK_volume` PRIMARY KEY (`VolumeId`);
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'volume', 'VolumeId');

ALTER TABLE `tipotratamento` ADD CONSTRAINT `PK_tipotratamento` PRIMARY KEY (`TipoTratamentoId`);
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'tipotratamento', 'TipoTratamentoId');

ALTER TABLE `tipoobra` ADD CONSTRAINT `PK_tipoobra` PRIMARY KEY (`TipoObraId`);
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'tipoobra', 'TipoObraId');

ALTER TABLE `tipojanela` ADD CONSTRAINT `PK_tipojanela` PRIMARY KEY (`TipoJanelaId`);
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'tipojanela', 'TipoJanelaId');

ALTER TABLE `sintoma` ADD CONSTRAINT `PK_sintoma` PRIMARY KEY (`SintomaId`);
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'sintoma', 'SintomaId');

ALTER TABLE `qualidadear` ADD CONSTRAINT `PK_qualidadear` PRIMARY KEY (`QualidadeArId`);
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'qualidadear', 'QualidadeArId');

ALTER TABLE `dadosgeral` ADD CONSTRAINT `PK_dadosgeral` PRIMARY KEY (`DadosGeralId`);
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'dadosgeral', 'DadosGeralId');

ALTER TABLE `construtivo` ADD CONSTRAINT `PK_construtivo` PRIMARY KEY (`ConstrutivoId`);
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'construtivo', 'ConstrutivoId');

ALTER TABLE `cliente` ADD CONSTRAINT `PK_cliente` PRIMARY KEY (`ClienteId`);
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'cliente', 'ClienteId');

ALTER TABLE `calculovolume` ADD CONSTRAINT `PK_calculovolume` PRIMARY KEY (`CalculoVolumeId`);
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'calculovolume', 'CalculoVolumeId');

ALTER TABLE `calculovolume` ADD CONSTRAINT `FK_calculovolume_volume_VolumeId` FOREIGN KEY (`VolumeId`) REFERENCES `volume` (`VolumeId`) ON DELETE CASCADE;

ALTER TABLE `cliente` ADD CONSTRAINT `FK_cliente_Localidades_LocalidadeId` FOREIGN KEY (`LocalidadeId`) REFERENCES `Localidades` (`LocalidadeId`) ON DELETE CASCADE;

ALTER TABLE `construtivo` ADD CONSTRAINT `FK_construtivo_proposta_PropostaId` FOREIGN KEY (`PropostaId`) REFERENCES `proposta` (`PropostaId`) ON DELETE CASCADE;

ALTER TABLE `dadosgeral` ADD CONSTRAINT `FK_dadosgeral_proposta_PropostaId` FOREIGN KEY (`PropostaId`) REFERENCES `proposta` (`PropostaId`) ON DELETE CASCADE;

ALTER TABLE `dadosgeral` ADD CONSTRAINT `FK_dadosgeral_tipojanela_DgTipoJanelaId` FOREIGN KEY (`DgTipoJanelaId`) REFERENCES `tipojanela` (`TipoJanelaId`) ON DELETE CASCADE;

ALTER TABLE `dadosgeral` ADD CONSTRAINT `FK_dadosgeral_tipoobra_TipoObraId` FOREIGN KEY (`TipoObraId`) REFERENCES `tipoobra` (`TipoObraId`) ON DELETE CASCADE;

ALTER TABLE `dadosgeral` ADD CONSTRAINT `FK_dadosgeral_tipotratamento_TipoTratamentoId` FOREIGN KEY (`TipoTratamentoId`) REFERENCES `tipotratamento` (`TipoTratamentoId`) ON DELETE CASCADE;

ALTER TABLE `higrometria` ADD CONSTRAINT `FK_higrometria_dadosgeral_DadosGeralId` FOREIGN KEY (`DadosGeralId`) REFERENCES `dadosgeral` (`DadosGeralId`) ON DELETE CASCADE;

ALTER TABLE `higrometria` ADD CONSTRAINT `FK_higrometria_proposta_PropostaId` FOREIGN KEY (`PropostaId`) REFERENCES `proposta` (`PropostaId`) ON DELETE CASCADE;

ALTER TABLE `PasswordResetToken` ADD CONSTRAINT `FK_PasswordResetToken_utilizador_UtilizadorID` FOREIGN KEY (`UtilizadorID`) REFERENCES `utilizador` (`UtilizadorID`) ON DELETE CASCADE;

ALTER TABLE `proposta` ADD CONSTRAINT `FK_proposta_cliente_ClienteId` FOREIGN KEY (`ClienteId`) REFERENCES `cliente` (`ClienteId`) ON DELETE CASCADE;

ALTER TABLE `proposta` ADD CONSTRAINT `FK_proposta_utilizador_UtilizadorId` FOREIGN KEY (`UtilizadorId`) REFERENCES `utilizador` (`UtilizadorID`) ON DELETE CASCADE;

ALTER TABLE `qualidadear` ADD CONSTRAINT `FK_qualidadear_dadosgeral_DadosGeralId` FOREIGN KEY (`DadosGeralId`) REFERENCES `dadosgeral` (`DadosGeralId`) ON DELETE CASCADE;

ALTER TABLE `qualidadear` ADD CONSTRAINT `FK_qualidadear_volume_VolumeId` FOREIGN KEY (`VolumeId`) REFERENCES `volume` (`VolumeId`) ON DELETE CASCADE;

ALTER TABLE `sintoma` ADD CONSTRAINT `FK_sintoma_dadosgeral_DadosGeralId` FOREIGN KEY (`DadosGeralId`) REFERENCES `dadosgeral` (`DadosGeralId`) ON DELETE CASCADE;

ALTER TABLE `sintoma` ADD CONSTRAINT `FK_sintoma_proposta_PropostaId` FOREIGN KEY (`PropostaId`) REFERENCES `proposta` (`PropostaId`) ON DELETE CASCADE;

ALTER TABLE `utilizador` ADD CONSTRAINT `FK_utilizador_role_RoleID` FOREIGN KEY (`RoleID`) REFERENCES `role` (`RoleID`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250620111216_RemoveStringLocalidadeFromCliente', '7.0.11');

DROP PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`;

DROP PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS `POMELO_BEFORE_DROP_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID TINYINT(1);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `Extra` = 'auto_increment'
			AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS `POMELO_AFTER_ADD_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255), IN `COLUMN_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID INT(11);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
			AND `COLUMN_TYPE` LIKE '%int%'
			AND `COLUMN_KEY` = 'PRI';
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL AUTO_INCREMENT;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

ALTER TABLE `cliente` DROP FOREIGN KEY `FK_cliente_Localidades_LocalidadeId`;

ALTER TABLE `proposta` DROP FOREIGN KEY `FK_proposta_cliente_ClienteId`;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'cliente');
ALTER TABLE `cliente` DROP PRIMARY KEY;

ALTER TABLE `cliente` RENAME `Clientes`;

ALTER TABLE `Clientes` RENAME INDEX `IX_cliente_LocalidadeId` TO `IX_Clientes_LocalidadeId`;

ALTER TABLE `pais` ADD `CodigoIso` varchar(2) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '';

ALTER TABLE `Clientes` MODIFY COLUMN `Telefone2` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `Clientes` MODIFY COLUMN `Telefone1` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `Clientes` MODIFY COLUMN `NumeroFiscal` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `Clientes` ADD CONSTRAINT `PK_Clientes` PRIMARY KEY (`ClienteId`);
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'Clientes', 'ClienteId');

ALTER TABLE `Clientes` ADD CONSTRAINT `FK_Clientes_Localidades_LocalidadeId` FOREIGN KEY (`LocalidadeId`) REFERENCES `Localidades` (`LocalidadeId`) ON DELETE CASCADE;

ALTER TABLE `proposta` ADD CONSTRAINT `FK_proposta_Clientes_ClienteId` FOREIGN KEY (`ClienteId`) REFERENCES `Clientes` (`ClienteId`) ON DELETE CASCADE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250623140630_AddCodigoIsoToPai', '7.0.11');

DROP PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`;

DROP PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`;

COMMIT;

START TRANSACTION;

ALTER TABLE `Clientes` MODIFY COLUMN `Telefone2` bigint NULL;

ALTER TABLE `Clientes` MODIFY COLUMN `Telefone1` bigint NULL;

CREATE UNIQUE INDEX `IX_Cliente_NumeroFiscal_Unique` ON `Clientes` (`NumeroFiscal`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250623213641_NomeDaSuaMigracao', '7.0.11');

COMMIT;

START TRANSACTION;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250623213830_NifsValidate', '7.0.11');

COMMIT;

