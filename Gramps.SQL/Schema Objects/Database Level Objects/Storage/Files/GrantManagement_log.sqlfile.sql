ALTER DATABASE [$(DatabaseName)]
    ADD LOG FILE (NAME = [GrantManagement_log], FILENAME = 'E:\DB\GrantManagement_log.ldf', SIZE = 8192 KB, MAXSIZE = 2097152 MB, FILEGROWTH = 10 %);



