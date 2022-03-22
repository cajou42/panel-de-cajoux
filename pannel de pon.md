# pannel de pon 

## principe du jeu : 

puzzle game se déroulant dans dans une grille à la maniètre d'un tétris, des lignes composées de blocs de différentes couleurs surgissent du bas de la grille, le joueur dispose d'un curseur horizontale avec lequel il peut inverser la position de deux blocs, si trois blocs ou plus sont alignés,
ils disparaissent et le score augmente, les blocs au dessus sont soumis à la gravité. la partie s'arrête si un blocs touche la dernière ligne de la grille, c'est game over, le joueur entre son nom et il sera enregistrer dans une bdd avec le score.

## cahier des charges

 * connection à la bdd et entrer des informations dedans, (nom du joueur, score, rang, niveau atteint) (*difficulté 2*)
 * écran de jeu (écran titre, jeu, option, high score) (*difficulté 1*)
 * différentes option : changer les contrôles, changer la musique parmis une sélection, changer le theme parmis une sélection (*difficulté 2*)
 * faire en sorteque les blocs monte au fur et à mesure de la partie, et game over si le haut de la grille et atteint (*difficulté 3*)
 * faire un curseur horizontale que le joueur contrôle, lorqu'il appuie sur le button d'action, les blocs s'intervertissent (*difficulté 5-6*)
 * afficher la bdd dans la page high score (*difficulté 3*)
 * les blocs sont soumis à la gravité (*difficulté 4*)
 * augmentation de la vitesse par rapport au temps et niveau atteint (*difficulté 2*)
 * menu pause avec posibilité de reprendre ou quitter la partie (le score est enregistrer quand on quitte la partie, et sera indiquer par un champs spécial dans la bdd) (*difficulté 2*)
 * 3 tables -> compte, high score, bonus
 * certain bloc contiennent des bonus qui sont en lien avec la bdd bonus (*difficulté 2*)

