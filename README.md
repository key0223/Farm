<div align="center">
    <img width="794" height="446" alt="Image" src="https://github.com/user-attachments/assets/692b7738-b4ad-45c5-b31c-d65c11bea60b" alt="녹음 2024-11-09 143520">
</div>

<br/>

# 👩‍💻 Farming's My Vocation

+ **프로젝트 이름** : Farming's My Vocation
+ **기간** : 2025.12 - 2026.04
+ **개발 인원** : 1인
+ **개발 도구** : C#, Unity, Tiled (맵 에디터) 

<br/>

## 🛠️ Key Systems

### 1️⃣ 데이터 기반 아이템 팩토리 시스템
- **핵심 기술** : 팩토리 패턴, 추상 클래스, JSON 파싱
- **설명** : 아이템 생성 로직을 `ItemFactory`로 캡슐화. 새로운 아이템 추가 시 코드 수정 없이 JSON 데이터 추가만으로 즉시 게임에 반영되는 확장성 확보
```csharp
  public static Item Create(int id, int stack = 1)
  {
      if (!Exists(id))
      {
          Debug.LogWarning($"ItemFactory : 존재하지 않는 아이템 ID {id}");
          return null;
      }

      ItemDataBase data = TableDataManager.Instance.ItemDict[id];

      switch (data.ItemType)
      {
          case ItemType.OBJECTS:
              return new ObjectItem(data, stack);
          case ItemType.TOOLS:
              return CreateTool(data);
          case ItemType.WEAPONS:
              break;

          default:
              return null;
      }

      return null;
  }

Item item  = ItemFactory.Create(1000); /* ItemID */
 ```
<br/>

### 2️⃣ 커스텀 애니메이터 
- *[👉 See Animator Code](https://github.com/key0223/Farm/blob/ace10a4398ef7a5be835418b8913581f8268ec69/Assets/03.Scripts/Animation/AnimatedSprite.cs)*
- **핵심 기술** : 데이터 기반 애니메이션, 레이어 싱크
- **설명** : 유니티 메카님의 복잡한 상태 머신 대신, JSON 데이터를 기반으로 캐릭터 파츠(몸,머리,팔 등)를 실시간 동기화하여 재생하는 커스텀 애니메이터 구현
<div>
    <table>
  <tr>
    <td> <img width="306" height="275" alt="Image" src="https://github.com/user-attachments/assets/beffd85a-2d6f-4f8b-abaa-696824b8f26a" /></td>
    <td> <img width="565" height="275" alt="Image" src="https://github.com/user-attachments/assets/32f66cad-c0ba-437a-bcb2-2072ab9f95aa" /></td>
  </tr>
</table>
</div>

```csharp
 void UpdateLayer(int layerIndex)
 {
     if (_currentAnims[layerIndex] == null) return;

     AnimationData clip = _currentAnims[layerIndex];
     _layerTimers[layerIndex] += Time.deltaTime;


     if (_layerTimers[layerIndex] >= _interval)
     {
         NextFrame(layerIndex, clip);
         _layerTimers[layerIndex] = 0;
     }
 }
 ```
<br/>

### 3️⃣ 속성 바인딩 맵 시스템
- *[👉 See Map Code](https://github.com/key0223/Farm/tree/ace10a4398ef7a5be835418b8913581f8268ec69/Assets/03.Scripts/Map)*
- **핵심 기술** : Tiled Custom Properties, TileRuntimeFeature
- **설명** : 외부 맵 에디터의 커스텀 속성을 런타임 데이터와 직접 바인딩하여 타일별 속성(충돌,경작 가능 여부 등)을 자동화. Dictionary 구조를 통해 $O(1)$의 속도로 타일 데이터에 접근하며 상호작용 로직을 모듈화하여 관리
<div>
    <table>
  <tr>
    <td> <img width="226" height="224" alt="Image" src="https://github.com/user-attachments/assets/41153ea0-f933-4f1e-9261-a24b6c306707" /></td>
    <td><img width="500" height="224" alt="Image" src="https://github.com/user-attachments/assets/d0a9924a-b23b-43f0-bef7-9d110a487e9b" /></td>
  </tr>
</table>
</div>
<br/>

### 4️⃣ 다이얼로그 시스템
- *[👉 See Dialogue Code](https://github.com/key0223/Farm/blob/ace10a4398ef7a5be835418b8913581f8268ec69/Assets/03.Scripts/Dialogue/DialogueManager.cs)*
- **핵심 기술** : Regex Tag Parsing, State Management
- **설명** : 정규 표현식을 활용한 커스텀 태그 분석기로 복잡한 대사 분기를 제어. 게임 내 환경(시간,날짜)과 유저의 과거 선택 데이터를 조합하여 상황에 가장 적합한 대사를 추출.

<div>
    <table>
  <tr>
    <td> <img width="406" height="275" alt="Image" src="https://github.com/user-attachments/assets/b078db79-3c40-4fa1-b033-a8e70fd3026c" /> 대화 생성 툴 </td>
    <td> <img width="406" height="275" alt="Image" src="https://github.com/user-attachments/assets/af1c99d9-d9b6-4e48-9e1d-ee54d26af5e8" /> 태그 파싱 </td>
  </tr>
        <tr>
    <td> <img width="406" height="275" alt="Image" src="https://github.com/user-attachments/assets/cf76dbc4-5bd2-4b64-b827-c4f968b293d0" /> 대화 창 </td>
    <td> <img width="406" height="275" alt="Image" src="https://github.com/user-attachments/assets/d4eef295-413d-420c-8ce0-5e0062f323e1" /> 대화 창 </td>
  </tr>
</table>
</div>


<br/>


### 5️⃣ UI 프레임워크
- *[👉 See ClickableMenu Code](https://github.com/key0223/Farm/blob/ace10a4398ef7a5be835418b8913581f8268ec69/Assets/03.Scripts/UI/ClickableMenu.cs)*
- *[👉 See ClickableComponent Code](https://github.com/key0223/Farm/blob/ace10a4398ef7a5be835418b8913581f8268ec69/Assets/03.Scripts/UI/ClickableComponent.cs)*
- **핵심 기술** : Abstraction & Polymorphism
- **설명** : 클릭 가능한 모든 UI의 공통 동작을 추상 클래스화하고 규격화하여 다양한 UI에서 코드 재사용성을 극대화

```csharp
  /* ClickableMenu를 상속받은 클래스 */
  protected override void PerformHoverAction(Vector2 mousePos)
 {
     ClickableComponent previousHover = _currentClickableComponent;
     _currentClickableComponent = null;

     if (previousHover != null)
         previousHover.OnHoverExit();

     foreach (ClickableComponent component in _clickableComponents)
     {
         bool contains = component.ContainsPoint((int)mousePos.x, (int)mousePos.y);

         if (contains)
         {
             _currentClickableComponent = component;
             component.OnHover();

             ShopSlot slot = component.GetComponent<ShopSlot>();
             if (slot != null && slot.CurrentItem != null)
             {
                 UIManager.Instance.ShowTooltip(slot.CurrentItem, mousePos);
             }
             return;
         }
     }
 }
 ```
<img width="406" height="275" alt="Image" src="https://github.com/user-attachments/assets/a70efda5-98a5-4ab8-9065-9636f2ab88fd" />

*호버 시 툴팁이 뜨는 모습*
<br/>


