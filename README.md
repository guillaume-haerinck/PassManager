# TP1 Sécurité réseau UQAC

Gestionnaire de mot de passe sécurisé utilisable en ligne de commande.

## Prérequis

Il faut posséder .NET Core 2.0, et télecharger les packages de l'application en ligne de commande depuis le dossier src avec :
```
dotnet restore
```

## Utilisation
### Créer la base de donnée

Afin de commencer avec une base de donnée vierge, il faut créer la base depuis le dossier src :

```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Enregistrer des données

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

## Caractéristiques de sécurité

Les mots de passe sont sauvegardés en utilisant un chiffrement symétrique AES-256, sa clé est générée à partir du mot de passe maître avec l'algorithme PBKF2. Du sel aléatoire est de plus ajouté à chaque mot de passe.

## Dévellopé avec

* [.NET core 2.0](https://www.microsoft.com/net/learn/get-started/windows) - Le framework utilisé

## Auteurs

* **Guillaume Haerinck** 
* **Azis Tekaya**
* **Alexandre Noret**

## License

Ce projet est licencié sous la licence MIT
