using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundLegends.Lobby
{
    /// <summary>
    /// Supplies the ready-made UI pages for the lobby package. The player only needs to
    /// connect the real Character/Item/Server systems and provide the chosen intro video
    /// and character meshes. The pages, navigation, cards, rows, buttons and states exist
    /// automatically.
    /// </summary>
    public class SL_ReadyMadeUI : MonoBehaviour
    {
        public bool buildAutomatically = true;
        public bool hideLegacyPlaceholderPages = true;
        public bool useReferenceImageAsPageBackdrop = true;
        private Canvas canvas;
        private GameObject lobby;
        private Sprite backgroundSprite;
        private readonly Dictionary<string, GameObject> pages = new Dictionary<string, GameObject>();
        private Font font;

        private readonly string[] pageIds = {
            "STORE","LUCK ROYALE","CHARACTER","INVENTORY","SKILLS","EQUIPMENT","QUESTS","EVENTS",
            "VAULT","PET","COLLECTION","RANK","FRIENDS","MAIL","PROFILE","PARTY","GUILD","CHAT",
            "SETTINGS","EMOTES","ENTRANCE EMOTES","MODE SELECTION"
        };

        private void Start()
        {
            if (buildAutomatically) Build();
        }

        public void Build()
        {
            canvas = GetComponentInChildren<Canvas>(true);
            if (canvas == null) return;
            var lobbyTransform = canvas.transform.Find("Main3DLobby"); lobby = lobbyTransform != null ? lobbyTransform.gameObject : null;
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            backgroundSprite = LoadSprite("UI_Background");
            foreach (var id in pageIds)
                if (!pages.ContainsKey(id)) pages[id] = BuildPage(id);
            WireLobbyButtons();
            if (hideLegacyPlaceholderPages) HideLegacyPages();
        }

        private Sprite LoadSprite(string name)
        {
            var t = Resources.Load<Texture2D>(name);
            if (t == null) return null;
            return Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(.5f,.5f), 100f);
        }

        private void WireLobbyButtons()
        {
            foreach (var id in new[]{"STORE","LUCK ROYALE","CHARACTER","INVENTORY","SKILLS","EQUIPMENT","QUESTS","EVENTS","VAULT","PET","COLLECTION","RANK"})
            {
                var b = FindButton(id);
                if (b != null) { string key=id; b.onClick.AddListener(()=>Open(key)); }
            }
            var emotes=FindButton("EMOTES"); if(emotes!=null) emotes.onClick.AddListener(()=>Open("EMOTES"));
            var friends=FindButton("FRIENDS"); if(friends!=null) friends.onClick.AddListener(()=>Open("FRIENDS"));
            var mail=FindButton("MAIL"); if(mail!=null) mail.onClick.AddListener(()=>Open("MAIL"));
            var settings=FindButton("SETTINGS"); if(settings!=null) settings.onClick.AddListener(()=>Open("SETTINGS"));
            var selectMode=FindButton("SELECT MODE"); if(selectMode!=null) selectMode.onClick.AddListener(()=>Open("MODE SELECTION"));
            var entrance=FindButton("ENTRANCE EMOTES"); if(entrance!=null) entrance.onClick.AddListener(()=>Open("ENTRANCE EMOTES"));
        }

        private Button FindButton(string name)
        {
            var trs=canvas.GetComponentsInChildren<Transform>(true);
            foreach(var t in trs) if(t.name==name){var b=t.GetComponent<Button>();if(b!=null)return b;}
            return null;
        }

        private void HideLegacyPages()
        {
            foreach(var t in canvas.GetComponentsInChildren<Transform>(true))
            {
                if(t==canvas.transform) continue;
                var n=t.name;
                if(n.StartsWith("SystemPanel_") || n=="Settings" || n=="Emotes" || n=="ModeSelection") t.gameObject.SetActive(false);
            }
        }

        public void Open(string id)
        {
            foreach(var kv in pages) if(kv.Value!=null) kv.Value.SetActive(kv.Key==id);
            if(lobby!=null) lobby.SetActive(false);
            if(id=="MODE SELECTION") RefreshModes(pages[id].transform);
        }

        public void BackToLobby()
        {
            foreach(var kv in pages) if(kv.Value!=null) kv.Value.SetActive(false);
            if(lobby!=null) lobby.SetActive(true);
        }

        private GameObject BuildPage(string id)
        {
            var p=new GameObject("READY_PAGE_"+id.Replace(" ","_"),typeof(RectTransform),typeof(Image));
            p.transform.SetParent(canvas.transform,false); p.SetActive(false);
            var img=p.GetComponent<Image>(); img.color=new Color(.015f,.035f,.075f,.98f); img.sprite=backgroundSprite; img.type=Image.Type.Sliced;
            Stretch(p.GetComponent<RectTransform>());
            var header=Box(p.transform,"HEADER",new Vector2(.035f,.85f),new Vector2(.965f,.965f),new Color(.025f,.075f,.15f,.98f));
            Text(header.transform,id,30,TextAnchor.MiddleLeft,.03f,.05f,.75f,.95f);
            Button(header.transform,"BACK",new Vector2(.84f,.15f),new Vector2(.97f,.85f),BackToLobby);
            var sub=Subtitle(id); BoxText(p.transform,sub,new Vector2(.05f,.70f),new Vector2(.95f,.82f));
            BuildContent(p.transform,id);
            Button(p.transform,"BACK TO LOBBY",new Vector2(.38f,.055f),new Vector2(.62f,.125f),BackToLobby);
            return p;
        }

        private string Subtitle(string id)
        {
            switch(id){
                case "STORE": return "Featured items • currencies • categories • server-authoritative purchase flow";
                case "LUCK ROYALE": return "Free draw • ticket draw • normal draw • custom draw • Fate Vault • pity";
                case "CHARACTER": return "Ready-made character page; the Character System supplies the real mesh and data";
                case "INVENTORY": return "Search • filter • sort • item cards • quantities • details";
                case "SKILLS": return "Attack • Heal • Movement • Utility • elements • cooldown • level scaling";
                case "EQUIPMENT": return "Armory • weapon • armor • equipment slots • 3D preview";
                case "QUESTS": return "Main story • side quests • objectives • progress • rewards";
                case "EVENTS": return "Event banners • countdown • requirements • rewards • event environment";
                case "VAULT": return "Stored items • 3D item preview • rotation • zoom";
                case "PET": return "Pet selection • pet display • preview • pet system connection";
                case "COLLECTION": return "Characters • weapons • cosmetics • discoveries";
                case "RANK": return "Rank • battle power • leaderboard • competitive history";
                case "FRIENDS": return "Online friends • requests • invites • social status";
                case "MAIL": return "System mail • rewards • event messages • claim";
                case "PROFILE": return "In-game identity • name • level • rank • stats • inheritance";
                case "PARTY": return "Party creation • invite • ready status • team connection";
                case "GUILD": return "Guild • members • activities • guild connection";
                case "CHAT": return "World • party • guild • private chat";
                case "SETTINGS": return "Graphics • audio • controls • HUD • gameplay • display • account";
                case "EMOTES": return "Lobby emotes • entrance emotes • in-game emotes";
                case "ENTRANCE EMOTES": return "Entrance animations can be used in the lobby and when entering matches";
                case "MODE SELECTION": return "Adventure World is the starting mode; other modes unlock according to progression";
                default: return "Soulbound Legends ready-made page";
            }
        }

        private void BuildContent(Transform parent,string id)
        {
            var content=Box(parent,"CONTENT",new Vector2(.055f,.17f),new Vector2(.945f,.67f),new Color(.015f,.045f,.09f,.86f));
            switch(id)
            {
                case "SETTINGS": BuildSettings(content.transform); break;
                case "MODE SELECTION": BuildModes(content.transform); break;
                case "EMOTES": BuildEmotes(content.transform,false); break;
                case "ENTRANCE EMOTES": BuildEmotes(content.transform,true); break;
                case "STORE": Rows(content,"Featured Store","Gold 245,680","Diamonds 5,320","Aether 0","Premium hidden before Lv10"); break;
                case "LUCK ROYALE": Rows(content,"Draw Library","Free Draw","Ticket Draw","Normal Draw","Custom Draw","Fate Vault","Pity / Guarantee"); break;
                case "CHARACTER": Rows(content,"Character Creation / Preview","Male / Female","Face / Head / Hair","Body Size","Shared Skeleton","3D Preview Camera","Save / Reset / Randomize"); break;
                case "INVENTORY": Rows(content,"Inventory","Materials","Consumables","Quest Items","Equipment","Search / Filter / Sort"); break;
                case "SKILLS": Rows(content,"Skill Book","Attack","Heal","Movement","Utility","Elements","Cooldown","Level Scaling","Effects / Buffs / Debuffs"); break;
                case "EQUIPMENT": Rows(content,"Armory","Weapon","Armor","Equipment Slots","3D Character","3D Weapon","Rotation / Zoom / Lighting"); break;
                case "QUESTS": Rows(content,"Quest Log","Main Story","Side Quests","Objectives","Progress","Rewards","Markers"); break;
                case "EVENTS": Rows(content,"Events","Banner","Countdown","Start / End","Requirements","Rewards","Environment Profile"); break;
                case "VAULT": Rows(content,"Vault","Stored Items","Weapons","Equipment","Collectibles","3D Preview","Rotation / Zoom"); break;
                case "PET": Rows(content,"Pet","Pet Selection","3D Display","Animation","Pet Effects","Pet System"); break;
                case "COLLECTION": Rows(content,"Collection","Characters","Weapons","Cosmetics","Discoveries","Completion"); break;
                case "RANK": Rows(content,"Rank","Current Rank","Battle Power","Leaderboard","Season","History"); break;
                case "FRIENDS": Rows(content,"Friends","Online","Offline","Requests","Add Player","Party Invite"); break;
                case "MAIL": Rows(content,"Mail","System Messages","Rewards","Event Mail","Claim","Delete","Unread"); break;
                case "PROFILE": Rows(content,"Profile","In-Game Name","Level","Rank","Stats","Equipment","Inheritance"); break;
                case "PARTY": Rows(content,"Party","Create Party","Invite","Ready","Members","Team Connection"); break;
                case "GUILD": Rows(content,"Guild","Guild Info","Members","Activities","Guild Chat","Guild System"); break;
                case "CHAT": Rows(content,"Chat","World","Party","Guild","Private","Mute / Report"); break;
                default: Rows(content,id,"Ready-made layout","Live system connection","Server data","Buttons and page graphics included"); break;
            }
        }

        private void Rows(GameObject parent,string title,params string[] rows)
        {
            Text(parent.transform,title,24,TextAnchor.MiddleLeft,.03f,.86f,.97f,.99f);
            float top=.78f; float step=Mathf.Min(.105f, .70f/Mathf.Max(1,rows.Length));
            for(int i=0;i<rows.Length;i++){var r=Box(parent.transform,"ROW_"+i,new Vector2(.035f,top-step*(i+1)),new Vector2(.965f,top-step*i),new Color(.025f,.075f,.14f,.96f));Text(r.transform,rows[i],17,TextAnchor.MiddleLeft,.025f,.02f,.72f,.98f);Button(r.transform,"OPEN",new Vector2(.76f,.12f),new Vector2(.97f,.88f),()=>{});}
        }

        private void BuildSettings(Transform parent)
        {
            string[] cats={"GRAPHICS","AUDIO","CONTROLS","HUD","GAMEPLAY","DISPLAY","ACCOUNT"};
            for(int i=0;i<cats.Length;i++){string c=cats[i];Button(parent,c,new Vector2(.10f,.78f-i*.095f),new Vector2(.90f,.86f-i*.095f),()=>ShowToast(c+" settings page ready"));}
        }

        private void BuildEmotes(Transform parent,bool entrance)
        {
            string prefix=entrance?"ENTRANCE EMOTE ":"EMOTE ";
            for(int i=0;i<6;i++){int n=i;Button(parent,prefix+(i+1),new Vector2(.08f+(i%3)*.30f,.55f-(i/3)*.25f),new Vector2(.32f+(i%3)*.30f,.70f-(i/3)*.25f),()=>ShowToast("Previewing "+prefix+(n+1)));}
        }

        private void BuildModes(Transform parent)
        {
            string[] names={"ADVENTURE WORLD","CLASH / CS","BATTLE ROYALE","SIEGE"}; string[] desc={"Learn & Discover","Practice & Sharpen","Adapt & Survive","Coordinate & Execute"};
            for(int i=0;i<4;i++){int n=i;var card=Box(parent.transform,"MODE_"+i,new Vector2(.06f,.74f-i*.16f),new Vector2(.94f,.86f-i*.16f),new Color(.025f,.08f,.16f,.98f));Text(card.transform,names[i]+"\n"+desc[i],18,TextAnchor.MiddleLeft,.03f,.05f,.68f,.95f);Button(card.transform,i==0?"SELECT":"LOCKED • LV10",new Vector2(.70f,.18f),new Vector2(.96f,.82f),()=>ShowToast(i==0?"Adventure World selected":"Reach Lv10 to unlock"));}}

        private void RefreshModes(Transform page){}

        private GameObject Box(Transform parent,string name,Vector2 min,Vector2 max,Color color){var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);var rt=go.GetComponent<RectTransform>();rt.anchorMin=min;rt.anchorMax=max;rt.offsetMin=rt.offsetMax=Vector2.zero;var im=go.GetComponent<Image>();im.color=color;im.sprite=backgroundSprite;im.type=Image.Type.Sliced;return go;}
        private void BoxText(Transform parent,string value,Vector2 min,Vector2 max){var b=Box(parent,"SUBTITLE",min,max,new Color(.05f,.12f,.22f,.95f));Text(b.transform,value,18,TextAnchor.MiddleLeft,.03f,.02f,.97f,.98f);}
        private Text Text(Transform parent,string value,int size,TextAnchor align,float x1,float y1,float x2,float y2){var go=new GameObject("Text",typeof(RectTransform),typeof(UnityEngine.UI.Text));go.transform.SetParent(parent,false);var t=go.GetComponent<UnityEngine.UI.Text>();t.font=font;t.text=value;t.fontSize=size;t.alignment=align;t.color=Color.white;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;var rt=go.GetComponent<RectTransform>();rt.anchorMin=new Vector2(x1,y1);rt.anchorMax=new Vector2(x2,y2);rt.offsetMin=rt.offsetMax=Vector2.zero;return t;}
        private Button Button(Transform parent,string title,Vector2 min,Vector2 max,UnityEngine.Events.UnityAction action){var go=new GameObject(title,typeof(RectTransform),typeof(Image),typeof(Button));go.transform.SetParent(parent,false);var rt=go.GetComponent<RectTransform>();rt.anchorMin=min;rt.anchorMax=max;rt.offsetMin=rt.offsetMax=Vector2.zero;var im=go.GetComponent<Image>();im.color=new Color(.035f,.10f,.20f,.98f);var b=go.GetComponent<Button>();b.targetGraphic=im;b.onClick.AddListener(action);var c=b.colors;c.normalColor=new Color(.035f,.10f,.20f,.98f);c.highlightedColor=new Color(.10f,.35f,.75f,1);c.pressedColor=new Color(.45f,.70f,1,1);c.selectedColor=c.highlightedColor;c.disabledColor=new Color(.15f,.15f,.18f,.7f);b.colors=c;Text(go.transform,title,16,TextAnchor.MiddleCenter,.02f,.02f,.98f,.98f);return b;}
        private void Stretch(RectTransform rt){rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=rt.offsetMax=Vector2.zero;}
        private void ShowToast(string message){Debug.Log("Soulbound Legends UI: "+message);}
    }
}
