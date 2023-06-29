using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private MovementManager player; // player

    [Header("Skins")]
    [HideInInspector] public int equippedSkin; // currently active skin
    [SerializeField] private PlayerSkin[] Skins; // all player skins

    /// <summary>
    /// Makes stuff to prepare player for start of the game.
    /// </summary>
    public void StartOfLevel()
    {
        player = MovementManager.instance;
        GetComponent<UserInput>().UpdatePlayerInstance(player);

        SyncPlayerSkin();
    }

    /// <summary>
    /// Sets player's skin and other effects.
    /// </summary>
    private void SyncPlayerSkin()
    {
        player.GetComponent<SpriteRenderer>().sprite = Skins[equippedSkin].sprite;
        player.GetComponentInChildren<TrailRenderer>().colorGradient = Skins[equippedSkin].color;
        player.GetComponentInChildren<TrailRenderer>().time = Skins[equippedSkin].time;

        Color dieCol = Skins[equippedSkin].dieColor;

        var mainPs = player.DeathParticles.GetComponent<ParticleSystem>().main; // main death particle system
        mainPs.startColor = dieCol;

        var c = player.DeathParticles.GetComponentsInChildren<ParticleSystem>()[1].colorOverLifetime; // splash particles
        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(dieCol, 0.0f), new GradientColorKey(dieCol, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 0.8f), new GradientAlphaKey(0.0f, 1.0f) });
        c.color = grad;
    }

    /// <summary>
    /// Pauses player's movement audio.
    /// </summary>
    public void pausePlayerAudio()
    {
        player.GetComponentsInChildren<AudioSource>()[1].Pause();
    }

    /// <summary>
    /// Unpauses player's movement audio.
    /// </summary>
    public void continuePlayerAudio()
    {
        player.GetComponentsInChildren<AudioSource>()[1].Play();
    }

    /// <summary>
    /// Starts the level when user touches the screen.
    /// </summary>
    public void StartFromTap()
    {
        player.StartFromTap();
    }

    /// <summary>
    /// Respawns player on last checkpoint.
    /// </summary>
    public void RespawnOnLastCheckpoint()
    {
        player.RespawnOnLastCheckpoint();
    }

    /// <summary>
    /// Disables player if he exists.
    /// </summary>
    public void DisablePlayerIfEnabled()
    {
        if (player != null) player.gameObject.SetActive(false);
    }
}
