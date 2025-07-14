using CourseVoiture;
namespace TestCourseVoiture;

/// <summary>
/// Tests des déplacements (voiture qui bouge dans le circuit qui se décale)
/// </summary>
[TestClass]
[DoNotParallelize]
public class TestDeplacements
{
    // A remplir
    private readonly char[] RouteParcours = { 'R' }; // Define RouteParcours with a default value
// Teste la méthode AvancerRoute décale correctement les lignes du parcours
    [TestMethod]
    public void AvancerRoute_DecaleTableau()
    {
        var course = new Course();
        // Prépare un parcours identifiable
        course.parcours = new string[] { "AAA", "BBB", "CCC" };
        // Définir HauteurJeu=3 LargeurJeu=3 temporairement impossible, mais test logique
        course.AvancerRoute();
        Assert.AreEqual("CCC", course.parcours[0], "La première ligne doit être l'ancienne dernière");
    }

    // Teste que la longueur du parcours reste constante après décalage
    [TestMethod]
    public void AvancerRoute_ConserveLongueurParcours()
    {
        var course = new Course();
        int longueurInitiale = course.parcours.Length;

        course.AvancerRoute();

        Assert.AreEqual(longueurInitiale, course.parcours.Length);
    }
}