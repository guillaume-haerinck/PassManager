# TP1 Sécurité réseau UQAC

Gestionnaire de mot de passe sécurisé utilisable en ligne de commande.

## Prérequis

Il faut posséder .NET Core 2.0, et télecharger les packages de l'application en ligne de commande depuis le dossier src avec :
```
dotnet restore
```

## Créer la base de donnée

Afin de commencer avec une base de donnée vierge, il faut créer la base depuis le dossier src :

```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Utiliser l'application

Une fois la base de donnée crée, l'application peut répondre aux entrées suivantes :

Pour enregistrer un utilisateur
```
dotnet run -r USERNAME MASTER_PASSWORD
```

Pour ajouter un mot de passe
```
dotnet run -a USERNAME MASTER_PASSWORD TAG PASSWORD
```

Pour récuperrer un mot de passe
```
dotnet run -g USERNAME MASTER_PASSWORD TAG
```

Pour supprimer un mot de passe
```
dotnet run -d USERNAME MASTER_PASSWORD TAG
```

## Dévellopé avec

* [.NET core 2.0](https://www.microsoft.com/net/learn/get-started/windows) - Le framework utilisé

## Autheurs

* **Guillaume Haerinck** 
* **Azis Tekaya**
* **Alexandre Noret**

## License

Ce projet est licencié sous la licence MIT