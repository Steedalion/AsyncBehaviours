using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncBehaviours
{
    public class AsyncCooldown
    {
        public bool IsCooldown { get; private set; }
        private readonly TimeSpan cooldown;
        private readonly Action action;
        private readonly Action cancelled;

        public AsyncCooldown(TimeSpan cooldown, Action action)
        {
            this.cooldown = cooldown;
            this.action = action;
        }

        public AsyncCooldown(TimeSpan cooldown, Action action, Action cancelled)
        {
            this.cancelled = cancelled;
            this.cooldown = cooldown;
            this.action = action;
        }

        /// <summary>
        /// Awaiting this will await the cooldown.
        /// Sometimes it is better not to await.
        /// </summary>
        public async Task Fire(CancellationToken token=default)
        {
            if (IsCooldown)
            {
                cancelled?.Invoke();
                return;
            }

            IsCooldown = true;
            action.Invoke();
            await Task.Delay(cooldown);
            IsCooldown = false;
            token.ThrowIfCancellationRequested();
        }

        // public void Fire()
        // {
        //     Fire();
        // }
    }
}