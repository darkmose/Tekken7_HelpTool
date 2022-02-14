using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIElement : MonoBehaviour
{
    [SerializeField] private Text _indexText;
    [SerializeField] private Image _charImage;
    [SerializeField] private Image _deadImage;
    [SerializeField] private Transform _winCountTransform;

    public CharacterUIElement ClearWinCount() 
    {
        var count = _winCountTransform.childCount;
        for (int i = 0; i < count; i++)
        {
            _winCountTransform.GetChild(0).gameObject.SetActive(false);
            _winCountTransform.GetChild(0).SetParent(ObjectPooler.PooledObjectsRoot);
        }
        return this;
    }

    public CharacterUIElement SetIndex(int index) 
    {
        _indexText.text = index.ToString() + ".";
        return this;
    }

    public CharacterUIElement SetCharImage(Sprite sprite) 
    {
        _charImage.sprite = sprite;
        return this;
    }

    public CharacterUIElement SetDead(bool isDead) 
    {
        _deadImage.enabled = isDead;
        return this;
    }

    public CharacterUIElement AddPerfectWin(int count) 
    {
        for (int i = 0; i < count; i++)
        {
            var v = ObjectPooler.GetPooledGameObject("V");
            v.transform.SetParent(_winCountTransform);
            v.GetComponent<Image>().color = Color.yellow;
            v.transform.localScale = Vector3.one * 1.5f;
        }
        return this;
    }

    public CharacterUIElement AddWin(int count) 
    {
        for (int i = 0; i < count; i++)
        {
            var v = ObjectPooler.GetPooledGameObject("V");
            v.transform.SetParent(_winCountTransform);
            v.GetComponent<Image>().color = Color.green;
            v.transform.localScale = Vector3.one;
        }
        return this;
    }

    private void OnDestroy()
    {
        //Clear data;
    }
}
