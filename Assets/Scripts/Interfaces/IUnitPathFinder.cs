public interface IUnitPathFinder
{
    bool CanMoveThroughPath { get; set; }
    RingPosition CurrentRingPosition { get; }
    void SetRingPosition(RingPosition ringPosition);
    void Tick();
}