# Hotfix — 이름 충돌 없이 붙이는 보조 스크립트

## 필수 조치
- **아래 두 파일이 존재하면 삭제하세요.** (이번 충돌의 원인)
  - `Assets/_Game/Scripts/PlayerMove.cs`
  - `Assets/_Game/Scripts/PlayerFire.cs`
  (기존 프로젝트의 `PlayerMove` / `PlayerFire`는 그대로 두세요.)

## 이 패키지에서 제공하는 것
1) `PlayerMoveExtensions.cs`
   - `PlayerMove.Heal(int)` 확장 메서드 제공
   - 기존 `PlayerMove`에 `Heal`이 이미 있다면 **원래 메서드가 우선**이라 충돌하지 않음
   - `CurrentHP` / `MaxHP` 필드명을 리플렉션으로 찾아 적용 (없으면 경고만 출력)

2) `PlayerHitCounter.cs`
   - 플레이어에 추가로 붙이면 **적과 3번 충돌 시 사망** 로직을 독립적으로 처리
   - `PlayerMove.Die()` 메서드가 있으면 호출, 없으면 GameObject 파괴

## 사용 방법
- 이 패키지를 프로젝트 루트에 병합
- 플레이어 오브젝트에 `PlayerHitCounter`를 추가
- (선택) `Item.cs`에서 호출하는 `Heal()`이 필요하면 이 확장 메서드가 자동으로 커버
  - 인스펙터/코드에는 변경 필요 없음

## 주의
- 충돌 오류(CS0101/CS0111)가 다시 보이면, 중복 클래스 파일이 남아 있는지 확인하세요.
