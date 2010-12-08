ALTER DATABASE [$(DatabaseName)]
    ADD LOG FILE (NAME = [GrantManagement_log], FILENAME = '$(DefaultLogPath)GrantManagement_log.ldf', MAXSIZE = 2097152 MB, FILEGROWTH = 10 %);

