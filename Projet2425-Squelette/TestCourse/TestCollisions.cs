using CourseVoiture;
namespace TestCourseVoiture;

/// <summary>
/// Tests de détection des collisions entre la voiture et le décor
/// </summary>
[TestClass]
[DoNotParallelize]
public class TestCollisions
{
    // Teste si une voiture hors des bords (à gauche) déclenche une collision
    [TestMethod]
    public void DeplacerVoiture_HorsBords_DetecteCollision()
    {
        var course = new Course();
        
        // Prépare un parcours vide (sans murs) de la taille du jeu
        course.parcours = new string[Course.HauteurJeu];
        string vide = new string(' ', Course.LargeurJeu);
        for (int i = 0; i < Course.HauteurJeu; i++) course.parcours[i] = vide;

        // Place la voiture complètement hors de l'écran à gauche
        // (bord gauche de la route étant à 22 si largeur = 70 et route = 26)
        course.positionVoiture = -1;
        course.PartieEnCours = true;

        // Appelle le déplacement ; doit détecter une sortie du parcours
        course.DeplacerVoiture();

        // Vérifie que la partie est arrêtée après collision
        Assert.IsFalse(course.PartieEnCours, "PartieEnCours doit devenir false en cas de collision");
    }

    // Teste si une sortie de la route par la droite déclenche une collision
    [TestMethod]
    public void DeplacerVoiture_SortieRouteDroite_DetecteCollision()
    {
        var course = new Course();

        // Place la voiture juste à droite du mur droit du parcours (collision attendue)
        course.positionVoiture = course.parcours[0].LastIndexOf(Course.MurParcours) + 1;
        course.PartieEnCours = true;

        course.DeplacerVoiture();

        // Vérifie que la collision a mis fin à la partie
        Assert.IsFalse(course.PartieEnCours);
    }

    // Teste si une sortie de la route par la gauche déclenche une collision
    [TestMethod]
    public void DeplacerVoiture_SortieRouteGauche_DetecteCollision()
    {
        var course = new Course();

        // Place la voiture juste à gauche du mur gauche du parcours (collision attendue)
        course.positionVoiture = course.parcours[0].IndexOf(Course.MurParcours) - 1;
        course.PartieEnCours = true;

        course.DeplacerVoiture();

        // Vérifie que la collision a mis fin à la partie
        Assert.IsFalse(course.PartieEnCours);
    }

}