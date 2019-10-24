using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(TransitionSystemGroup))]
class WalkTransitionSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new ApplyWalkTransition
        {
            commandBuffer = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var handle = job.Schedule(this, inputDeps);
        m_CommandBufferSystem.AddJobHandleForProducer(handle);
        return handle;
    }

    [RequireComponentTag(typeof(WALK))]
    struct ApplyWalkTransition : IJobForEachWithEntity<PlatformId, CurrentPathIndex, PathLookup>
    {
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(Entity entity, int jobIndex,
            [ReadOnly] ref PlatformId platformId,
            [ReadOnly] ref CurrentPathIndex pathIndex,
            [ReadOnly] ref PathLookup pathLookup)
        {
            var path = pathLookup.value.Value.paths[pathIndex.pathLookupIdx];
            if (platformId.value == path.connections[pathIndex.index].destinationPlatformId)
            {
                commandBuffer.RemoveComponent<WALK>(jobIndex, entity);
                commandBuffer.AddComponent<QUEUE>(jobIndex, entity);
            }
        }
    }
}
