// INTENTIONAL NEGATIVE TEST DATA: this component is never mounted.
export default function UnusedLegacyBanner() {
  const unusedMessage = 'Legacy scholarship banner'
  const unusedCount = 7
  return (
    <aside>
      {unusedMessage} {unusedCount}
    </aside>
  )
}
