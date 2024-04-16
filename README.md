<h3 align="center">MerrMail</h3>

<!--suppress ALL-->
<p align="center">
    <img src="https://img.shields.io/github/license/merrsoft/merrmail?style=for-the-badge" alt="GitHub License">
    <img src="https://img.shields.io/github/stars/merrsoft/merrmail?style=for-the-badge" alt="GitHub Repo stars">
    <img src="https://img.shields.io/github/forks/merrsoft/merrmail?style=for-the-badge" alt="GitHub forks">
    <img src="https://img.shields.io/github/issues/merrsoft/merrmail?style=for-the-badge" alt="GitHub Issues or Pull Requests">
</p>

<p align="center">MerrMail is an open-source email bot/assistant designed to organize and automate replies to your common emails.</p>

### 🤔 How It Works?
MerrMail utilizes AI to analyze and generate replies to your emails by providing context. Once the program starts, it creates three labels within your email account and begins organizing email threads from your inbox. Importantly, it does not mark your emails as read, allowing you to keep track of which ones you have already reviewed. (Currently, it only supports Gmail).

- `MerrMail: High Priority` are email threads that did not receive an auto-reply.
- `MerrMail: Low Priority` are email threads that received a reply from MerrMail. The reply is generated using the email contexts provided by the user of this program.
- `MerrMail: Other` are email threads initiated by the host or email addresses identified as 'no-reply'.

### 🚀 Getting Started
Before you begin, make sure you have the following. Note that all of these are free.

- [Git](https://git-scm.com)
- [.NET 8](https://dotnet.microsoft.com/download)
- [Docker Compose](https://www.docker.com)
- [SQLite](https://www.sqlite.org) (optional but recommended)
- [Google App Password](https://support.google.com/accounts/answer/185833)
- [Gmail OAuth 2.0 Client Credentials](https://developers.google.com/gmail/api/guides)

### 🛠️ Installation
We will be building from source, so please follow the steps to install MerrMail on your system.

- First, open your terminal and clone this repository.
```sh
$ git clone https://github.com/merrsoft/merrmail.git 
$ cd merrmail
```

- Take a look at the docker-compose.yml file. Alternatively, can build the Docker image from source.
```yml
version: '3'

networks:
  merrnet:
    driver: bridge

services:
  tensorflow:
    build:
      context: src/Infrastructure
      dockerfile: Dockerfile
    ports:
      - "63778:63778"
    networks:
      - merrnet
    volumes:
      - ./secrets/universal_sentence_encoder:/universal_sentence_encoder
```

- Setup our custom container of TensorFlow's Universal Sentence Encoder.
```sh
$ mkdir -p secrets/universal_sentence_encoder 
$ docker-compose up -d
```

- Navigate to the main entrypoint of the program.
```sh
$ cd src/MerrMail 
```

### 🔧 Configuration
In order to run the program, you must properly set each configuration of MerrMail. You can also refer to our sample [settings](https://github.com/merrsoft/merrmail/blob/main/secrets.json).

- `EmailReplyOptions:Header` sets the header format of the email.
- `EmailReplyOptions:Introduction` sets the first part of the email. then shows the message from the database.
- `EmailReplyOptions:Conclusion` sets the last part of the email body.
- `EmailReplyOptions:Closing` is self-explanatory.
- `EmailReplyOptions:Signature` is the same.

```sh
$ dotnet user-secrets set EmailReplyOptions:Header "Greet the sender"
$ dotnet user-secrets set EmailReplyOptions:Introduction "Explain what's going on"
$ dotnet user-secrets set EmailReplyOptions:Conclusion "Outline next steps"
$ dotnet user-secrets set EmailReplyOptions:Closing "Regards"
$ dotnet user-secrets set EmailReplyOptions:Signature "Provide contact information"
```

<br>

- `EmailApiOptions:OAuthClientCrednetialsFilePath` is the location of your credentials.json file. Assuming you already downloaded the json credentials from your Gmail API.
- `EmailApiOptions:AccessTokenDirectoryPath` is the folder where the program will look for your access token. If nothing found, it will open a browser for you to give the program an access to your Gmail API, and then creates the access token.
- `EmailApiOptions:HostAddress` is your email address.
- `EmailApiOptions:HostPassword` is the 16-digit app password provided by Google. Assuming you already created an app password for your Google account.

```sh
$ dotnet user-secrets set EmailApiOptions:OAuthClientCredentialsFilePath "/path/to/your/credentials.json"
$ dotnet user-secrets set EmailApiOptions:AccessTokenDirectoryPath "/path/to/your/token_folder"
$ dotnet user-secrets set EmailApiOptions:HostAddress "your.email@sample.domain"
$ dotnet user-secrets set EmailApiOptions:HostPassword "your_email_account_app_password"
```

<br>

- `EmailAnalyzerOptions:AcceptanceScore 0.35` is the "cosine similarity score" that is accepted by your program (we recommend between 0.24 and 0.35). It doesn't allow values that's not between 0.0 and 1.0.
```sh
$ dotnet user-secrets set EmailAnalyzerOptions:AcceptanceScore 0.35
```

<br>

- `DataStorageOptions:DataStorageType 0` sets the type of your data storage to SQLite.
- `DataStorageOptions:DataStorageType 1` sets the data storge type to CSV. Yes, we support CSV! 🎉
- `DataStorageOptions:DataStorageAccess` is the location of your data storage file. The file type varies depending on what data storage type you chose.
```sh
$ dotnet user-secrets set DataStorageOptions:DataStorageType 0
$ dotnet user-secrets set DataStorageOptions:DataStorageAccess "/path/to/your/data/storage/file"
```


### 📑 Data Storage
Below is the format of your data storage depending of which type you chose.

- **email-context.csv**
```
Subject,Response
"The first row is not","Going to be read."
"Here is my subject","Here is my response, you can use comma."
We recommend wrapping values with quotes,"Wrap ""your quotes"" twice if you want to use quotes."
```

- **email-context.db**
```sql
-- Note that this is the create statement, not the content of the file.
-- Use SQLite Browser to create tables.
CREATE TABLE "EmailContext" (
	"Subject"	TEXT NOT NULL,
	"Response"	TEXT NOT NULL
)
```

### 📜 License
Before running our program, please refer to our [License](https://github.com/merrsoft/merrmail/blob/main/LICENSE).

### ▶️ Run
- You can check if you've properly configured all your app secrets.
```sh
$ dotnet user-secrets list
```

- If everything is configured correctly, you can now run the program!
```sh
$ dotnet run
```

### 🙏 Thanks!

Thank you for choosing MerrMail! We're excited to see how our open-source email bot transforms your inbox. If you have any questions or feedback, feel free to reach out to us. Happy emailing! 🌟
