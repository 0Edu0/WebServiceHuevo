-- Script de creacion de la base y la tabla principal
-- Ejecutar conectado al servidor MySQL 192.168.1.31:3306

CREATE DATABASE IF NOT EXISTS `proyectoFinalArqui1`
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE `proyectoFinalArqui1`;

DROP TABLE IF EXISTS Huevos;

CREATE TABLE Huevos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Peso DECIMAL(5,2),
    Categoria VARCHAR(50) NOT NULL,
    Color VARCHAR(50) NOT NULL,
    FechaIngreso DATETIME DEFAULT CURRENT_TIMESTAMP
);
