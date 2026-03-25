# 🍎 Food Explorer

**Food Explorer** est une application de bureau moderne pour Windows conçue pour aider les utilisateurs à mieux comprendre leur alimentation. En scannant ou en recherchant des produits, l'application offre un accès instantané aux informations nutritionnelles, aux additifs et aux scores de santé.

---

## 🎯 Objectif du Projet

L'objectif de **Food Explorer** est de promouvoir une **alimentation responsable et saine**. L'application transforme des données techniques complexes en informations visuelles simples pour permettre aux utilisateurs de faire des choix éclairés lors de leurs achats.



---

## 🚀 Fonctionnalités Clés

* **Scanner de Code-Barres :** Identifiez vos produits en temps réel via la webcam de votre ordinateur (technologie laser numérique).
* **Analyse Nutritionnelle Complète :** Visualisation immédiate du **Nutri-Score**, de la liste des ingrédients et des allergènes.
* **Détection des Additifs :** Alerte sur les substances controversées présentes dans les produits transformés.
* **Historique de Consultation :** Retrouvez automatiquement les derniers produits consultés, même sans connexion internet (sauvegarde locale).
* **Favoris & Notes Personnelles :** Créez votre propre liste de produits sains et ajoutez des commentaires personnalisés (ex: "Idéal pour le petit-déjeuner").
* **Génération de Rapports PDF :** Exportez vos sélections ou vos listes de favoris dans un document PDF professionnel pour un suivi diététique.
* **Données Mondiales :** Utilisation de la base de données collaborative [Open Food Facts](https://world.openfoodfacts.org/).

---

## 🏗️ Architecture Logicielle

Pour garantir la solidité et l'évolution du logiciel, **Food Explorer** repose sur des standards de développement professionnels :

* **Architecture Clean :** Séparation stricte entre l'interface visuelle et la logique de calcul.
* **Pattern MVVM (Model-View-ViewModel) :** Une structure qui permet une maintenance facile et une interface réactive.
* **Gestion de Données :** Utilisation d'une base de données locale **SQLite** pour la persistance des données utilisateur.
* **Modularité :** Utilisation d'interfaces et de services pour permettre d'ajouter de nouvelles fonctionnalités sans réécrire l'existant.



---

## 🛠️ Technologies Utilisées

* **Langage :** C# / .NET 10
* **Interface :** WPF (Windows Presentation Foundation) & XAML
* **Base de données :** Entity Framework Core & SQLite
* **Outils tiers :** ZXing (Scanner), iTextSharp (PDF), FlashCap (Caméra)


