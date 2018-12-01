using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Maps.Events
{
    public class TimedEventEmitter : MapEventEmitter
    {
        private float _intervalMin;
        private float _intervalMax;
        private int _repeat;
        public TimedEventEmitter(Map map, string eventKey, float intervalMin, float intervalMax, int repeat)
            : base(map, eventKey)
        {
            _intervalMin = intervalMin;
            _intervalMax = intervalMax;
            _repeat = repeat;
        }

        public override void onAddedToScene()
        {
            base.onAddedToScene();
            ScheduleNextEmit(initialSchedule: true);
        }

        private void ScheduleNextEmit(bool initialSchedule = false)
        {
            if (!initialSchedule)
            {
                EmitMapEvent();
            }

            if (_repeat > 0)
            {
                _repeat--;
            }
            else if (_repeat == 0)
            {
                return;
            }

            var timeSeconds = Nez.Random.range(_intervalMin, _intervalMax);
            Core.schedule(timeSeconds, _ => ScheduleNextEmit());
        }
    }
}
