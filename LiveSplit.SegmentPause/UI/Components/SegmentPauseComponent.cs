using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public class SegmentPauseComponent : LogicComponent, IDeactivatableComponent
    {
        public override string ComponentName => "Segment Pausing";

        public bool Activated { get; set; }

        private bool IsSplitting { get; set; }
        private int SplitCounter { get; set; }

        private LiveSplitState State { get; set; }
        private SegmentPauseSettings Settings { get; set; }
        private TimerModel Timer { get; set; }
        public SegmentPauseComponent(LiveSplitState state)
        {
            Activated = true;

            State = state;
            Settings = new SegmentPauseSettings();
            Timer = new TimerModel() { CurrentState = State };

            State.OnStart += State_OnStart;
            State.OnReset += State_OnReset;
        }

        public override void Dispose()
        {
            State.OnStart -= State_OnStart;
            State.OnReset -= State_OnReset;
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) { }

        public override Control GetSettingsControl(LayoutMode mode)
        {
            return Settings;
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public override void SetSettings(XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        private void State_OnStart(object sender, EventArgs e)
        {
            Time simulatedTime = Time.Zero;
            int previousAttempt = State.Run.AttemptHistory.Last().Index;
            for (int i = 0; i < State.Run.Count; i++)
            {
                ISegment currentSegment = State.Run[i];
                if (!currentSegment.SegmentHistory.ContainsKey(previousAttempt))
                {
                    // This split was not completed last segment.
                    break;
                }

                Time lastAttempt = currentSegment.SegmentHistory[previousAttempt];
                TimeSpan? lastTime = lastAttempt[State.CurrentTimingMethod];
                if (lastTime == null)
                {
                    // This split has no recorded time.
                    break;
                }

                simulatedTime[State.CurrentTimingMethod] += lastTime;
                if (simulatedTime[State.CurrentTimingMethod] > State.Run.Offset)
                {
                    // This is in the future!
                    break;
                }

                //timeRemaining = timeRemaining.Subtract((TimeSpan)lastTime);
                currentSegment.SplitTime = simulatedTime;
                Timer.SkipSplit();
                currentSegment.SplitTime = simulatedTime;
            }
        }

        protected void State_OnReset(object sender, TimerPhase phase)
        {
            Attempt lastAttempt = State.Run.AttemptHistory.Last();

            Time cumulativeTime = Time.Zero;
            foreach (var current in State.Run)
            {
                if (current.SegmentHistory.ContainsKey(lastAttempt.Index))
                {
                    cumulativeTime += current.SegmentHistory[lastAttempt.Index];
                    current.PersonalBestSplitTime = cumulativeTime;// current.SegmentHistory[lastAttempt.Index];
                }
                else
                {
                    // All done.
                    break;
                }
            }

            if (phase == TimerPhase.Ended)
            {
                State.Run.Offset = TimeSpan.Zero;
            }
            else if (lastAttempt.Duration != null)
            {
                State.Run.Offset += (TimeSpan)lastAttempt.Duration;
            }
        }

        /*
        // This internal component does the actual heavy lifting. Whenever we want to do something
        // like display text, we will call the appropriate function on the internal component.
        protected InfoTextComponent InternalComponent { get; set; }
        // This is how we will access all the settings that the user has set.
        public SegmentPauseSettings Settings { get; set; }
        // This object contains all of the current information about the splits, the timer, etc.
        protected LiveSplitState CurrentState { get; set; }

        public string ComponentName => "Reset Chance";

        public float HorizontalWidth => InternalComponent.HorizontalWidth;
        public float MinimumWidth => InternalComponent.MinimumWidth;
        public float VerticalHeight => InternalComponent.VerticalHeight;
        public float MinimumHeight => InternalComponent.MinimumHeight;

        public float PaddingTop => InternalComponent.PaddingTop;
        public float PaddingLeft => InternalComponent.PaddingLeft;
        public float PaddingBottom => InternalComponent.PaddingBottom;
        public float PaddingRight => InternalComponent.PaddingRight;

        // I'm going to be honest, I don't know what this is for, but I know we don't need it.
        public IDictionary<string, Action> ContextMenuControls => null;

        // This function is called when LiveSplit creates your component. This happens when the
        // component is added to the layout, or when LiveSplit opens a layout with this component
        // already added.
        public SegmentPauseComponent(LiveSplitState state)
        {

        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            InternalComponent.NameLabel.HasShadow
                = InternalComponent.ValueLabel.HasShadow
                = state.LayoutSettings.DropShadows;

            InternalComponent.NameLabel.ForeColor = state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel.ForeColor = state.LayoutSettings.TextColor;

            InternalComponent.DrawHorizontal(g, state, height, clipRegion);
        }

        // We will be adding the ability to display the component across two rows in our settings menu.
        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            //InternalComponent.DisplayTwoRows = Settings.Display2Rows;

            InternalComponent.NameLabel.HasShadow
                = InternalComponent.ValueLabel.HasShadow
                = state.LayoutSettings.DropShadows;

            InternalComponent.NameLabel.ForeColor = state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel.ForeColor = state.LayoutSettings.TextColor;

            InternalComponent.DrawVertical(g, state, width, clipRegion);
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            //Settings.Mode = mode;
            return Settings;
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        // This is the function where we decide what needs to be displayed at this moment in time,
        // and tell the internal component to display it. This function is called hundreds to
        // thousands of times per second.
        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {

        }

        // This function is called when the component is removed from the layout, or when LiveSplit
        // closes a layout with this component in it.
        public void Dispose()
        {

        }

        // I do not know what this is for.
        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();
        */
    }
}
